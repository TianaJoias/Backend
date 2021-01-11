using System;
using System.Collections.Generic;
using Domain.Portifolio;
using Domain.Stock;

namespace Domain.Catalog
{
    public class CatalogState
    {
        public enum States { Preparation, Ready, Sended, Delivered, Returned, Closing, Closed }
        public CatalogState(States state, int order)
        {
            State = state;
            Order = order;
            ChangedAt = Clock.Now;
        }
        public States State { get; set; }
        public DateTime ChangedAt { get; set; }
        public int Order { get; set; }
    }

    public class Catalog : BaseEntity
    {
        private readonly List<CatalogItem> _items;
        private readonly List<CatalogState> _changes;
        public CatalogState State { get; set; }

        protected Catalog()
        {
            _items = new List<CatalogItem>();
            _changes = new List<CatalogState>();
        }

        public Catalog(Agent channel) : this()
        {
            Agent = channel;
            CreatedAt = Clock.Now;
            ChangeState(CatalogState.States.Preparation);
        }

        private void ChangeState(CatalogState.States state)
        {
            var oldState = State;
            State = new CatalogState(state, _changes.Count);
            _changes.Add(State);
            AddEvent(new CatalogStateChangedEvent(this, oldState));
        }

        public Agent Agent { get; private set; }
        public IReadOnlyCollection<CatalogItem> Items => _items.AsReadOnly();
        public DateTime CreatedAt { get; private set; }
        public decimal TotalSold { get; set; }
        public void AddItem(Product produt, Lot lot, decimal quantity)
        {
            if (State.State == CatalogState.States.Closed) return;
            _items.Add(new CatalogItem(produt, lot, quantity));
            AddEvent(new ProductReservedEvent(quantity, lot.Id, produt.Id, lot.SalePrice, Agent.Id));
        }

        public void ReturnItem(Guid LotId, decimal quantity)
        {
            if (State.State == CatalogState.States.Closed) return;
            var item = _items.Find(it => it.LotId == LotId);
            var quantitySold = item.CurrentQuantity - quantity;
            item.Sell(quantitySold);
            item.Return(quantity);
            TotalSold += item.TotalSold;
            AddEvent(new ProductReturnedEvent(quantity, LotId, item.ProdutoId, item.Price, Agent.Id));
            AddEvent(new ProductConfirmedSaleEvent(quantitySold, LotId, item.ProdutoId, item.Price, Agent.Id));
        }

        public void CompleteClosing()
        {
            ChangeState(CatalogState.States.Closed);
        }

        public void CompletePreparing()
        {
            ChangeState(CatalogState.States.Ready);
        }

        public void Send()
        {
            ChangeState(CatalogState.States.Sended);
        }

        public void Delivery()
        {
            ChangeState(CatalogState.States.Delivered);
        }

        public void Return()
        {
            ChangeState(CatalogState.States.Returned);
        }

        public void StartClosing()
        {
            if (State.State != CatalogState.States.Closing)
                ChangeState(CatalogState.States.Closing);
        }
    }

    public class CatalogStateChangedEvent : BaseEvent
    {
        public CatalogStateChangedEvent(Catalog catalog, CatalogState previousState)
        {
            ChangedAt = catalog.State.ChangedAt;
            CurrentState = catalog.State.State;
            PreviousState = previousState.State;
            TotalSold = catalog.TotalSold;
            AgentId = catalog.Agent.Id;
        }

        public DateTime ChangedAt { get; }
        public CatalogState.States PreviousState { get; }
        public CatalogState.States CurrentState { get; }
        public decimal TotalSold { get; }
        public Guid AgentId { get; }
    }
}
