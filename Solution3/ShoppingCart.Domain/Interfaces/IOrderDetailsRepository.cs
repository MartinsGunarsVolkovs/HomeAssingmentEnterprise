﻿using ShoppingCart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Domain.Interfaces
{
    public interface IOrderDetailsRepository
    {
        int AddOrderDetails(OrderDetails od);
        void AddOrderDetailsList(List<OrderDetails> od);
    }
}
