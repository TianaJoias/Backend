using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Portifolio;
using Domain.Stock;
using Stateless;

namespace Domain.Catalog
{
    public class Catalog : BaseEntity
    {
        private readonly List<CatalogItem> _items;
        public enum States { Preparation, Ready, Sended, Delivered, Returned, Closing, Closed }

        public enum Trigger { Next, Close }

        public States State { get; private set; }
        public DateTime ChangedAt { get; private set; }
        public decimal ItemsQuantity { get; private set; }
        public decimal ItemsAddedQuantity { get; private set; }
        public decimal SoldQuantity { get; private set; }
        public Agent Agent { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public decimal SoldValue { get; private set; }
        public decimal ValuedAt { get; private set; }
        public IReadOnlyCollection<CatalogItem> Items => _items.AsReadOnly();

        private readonly StateMachine<States, Trigger> _stateMachine;
        protected Catalog()
        {
            _items = new List<CatalogItem>();

            _stateMachine = new StateMachine<States, Trigger>(() => State, s =>
            {
                ChangedAt = Clock.Now;
                State = s;
            });

            _stateMachine.Configure(States.Preparation)
                .Permit(Trigger.Next, States.Ready);

            _stateMachine.Configure(States.Ready)
                .Permit(Trigger.Next, States.Sended);

            _stateMachine.Configure(States.Sended)
                .Permit(Trigger.Next, States.Delivered);

            _stateMachine.Configure(States.Delivered)
                .Permit(Trigger.Next, States.Returned);

            _stateMachine.Configure(States.Returned)
                .Permit(Trigger.Next, States.Closing);

            _stateMachine.Configure(States.Closing)
                .Permit(Trigger.Close, States.Closed);

            _stateMachine.Configure(States.Closed)
                .OnEntry(() => OnClosed());
        }

        public Catalog(Agent channel) : this()
        {
            Agent = channel;
            CreatedAt = Clock.Now;
            ItemsQuantity = 0;
            State = States.Preparation;
            ChangedAt = Clock.Now;
        }

        private void OnClosed()
        {
            var itemsSold = _items.Where(it => it.CurrentQuantity > 0);
            foreach (var item in itemsSold)
            {
                var quantitySold = item.CurrentQuantity;
                item.Sell(quantitySold);
                SoldValue += item.ValueSold;
                SoldQuantity += item.QuantitySold;
                ItemsQuantity -= quantitySold;
                AddEvent(new ProductConfirmedSaleEvent(quantitySold, item.LotId, item.ProdutoId, item.Price, Agent.Id));
            }
        }

        public void AddItem(Product produt, Lot lot, decimal quantity)
        {
            if (State == States.Closed) return;
            var currentItem = _items.FirstOrDefault(it => it.LotId == lot.Id);
            if (currentItem is null)
            {
                _items.Add(new CatalogItem(produt, lot, quantity));
            }
            else
            {
                currentItem.AddInitialQuantity(quantity);
            }
            ItemsQuantity += quantity;
            ItemsAddedQuantity += quantity;
            ValuedAt += lot.SalePrice * quantity;
            AddEvent(new ProductReservedEvent(quantity, lot.Id, produt.Id, lot.SalePrice, Agent.Id));
        }

        public void ReturnItem(Guid LotId, decimal quantity)
        {
            if (State == States.Closed) return;
            var item = _items.Find(it => it.LotId == LotId);
            if (item.HasQuantity(quantity))
            {
                item.Return(quantity);
                ItemsQuantity -= quantity;
                AddEvent(new ProductReturnedEvent(quantity, LotId, item.ProdutoId, item.Price, Agent.Id));
            }
        }

        public void Next()
        {
            if (_stateMachine.CanFire(Trigger.Next))
                _stateMachine.Fire(Trigger.Next);
        }
        public void Close()
        {
            if (_stateMachine.CanFire(Trigger.Close))
                _stateMachine.Fire(Trigger.Close);
        }
        public void ChangeCatalog(Product product, Lot lot, decimal quantity, Catalog catalog)
        {
            if (State == States.Closed) return;
            var item = _items.Find(it => it.LotId == lot.Id);
            item.Return(quantity);
            catalog.AddItem(product, lot, quantity);
            ItemsQuantity -= quantity;
        }
    }


    public class CatalogStateChangedEvent : BaseEvent
    {
        public CatalogStateChangedEvent(Catalog catalog, Catalog.States previouState)
        {
            ChangedAt = catalog.ChangedAt;
            CurrentState = catalog.State;
            PreviousState = previouState;
            TotalSold = catalog.SoldValue;
            AgentId = catalog.Agent.Id;
        }

        public DateTime ChangedAt { get; }
        public Catalog.States? PreviousState { get; }
        public Catalog.States CurrentState { get; }
        public decimal TotalSold { get; }
        public Guid AgentId { get; }
    }
}
