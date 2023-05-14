﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Pizza_Ordering_System.Data;
using Online_Pizza_Ordering_System.Migrations;
using Online_Pizza_Ordering_System.Models;
using Online_Pizza_Ordering_System.Repositories;

namespace Online_Pizza_Ordering_System.Controllers
{
    //    [Authorize]
    public class CardPaymentController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ShoppingCart _shoppingCart;
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CardPaymentController(IOrderRepository orderRepository,
            ShoppingCart shoppingCart, AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _orderRepository = orderRepository;
            _shoppingCart = shoppingCart;
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult MakePayment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment([Bind("Id,username,CardNumber,CVC,ExpiryDate,Balance,OrderTotal,User")] CardPayment cardpayment)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            cardpayment.UserId = userId;
            var items = await _shoppingCart.GetShoppingCartItemsAsync();
            _shoppingCart.ShoppingCartItems = items;
            cardpayment.Balance = 0;
            if (_shoppingCart.ShoppingCartItems.Count == 0)
            {
                    ModelState.AddModelError("", "Your cart is empty, add some pizzas first");
            }

            //if (ModelState.IsValid)
            {
                    await _orderRepository.UpdateCardAsync(cardpayment);
                    await _shoppingCart.ClearCartAsync();
                
                return RedirectToAction("PaymentComplete");
            }

            return View("Index","ShoppingCart");
        }

        [HttpGet]
        public IActionResult AddCard()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCard([Bind("Id,username,CardNumber,CVC,ExpiryDate,Balance,OrderTotal")] CardPayment cardpayment)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            cardpayment.UserId = userId;

            //var items = await _shoppingCart.GetShoppingCartItemsAsync();
            //_shoppingCart.ShoppingCartItems = items;

          
                //if (ModelState.IsValid)
                //{
                    await _orderRepository.CreateCardAsync(cardpayment);
                //}

            return RedirectToAction("Index", "ShoppingCart");
        }
        public IActionResult PaymentComplete()
        {
            return  View();
        }


        // GET: Pizzas/Edit/5
        public async Task<IActionResult> Edit()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var card= await _context.CardPayment.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);
            
            //ViewData["StoreId"] = new SelectList(_storeRepo.GetAll(), "Id", "Name", pizzas.StoreId);
            return View(card);
        }

            //POST: Stores/Edit/
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(Guid id, [Bind("Id,username,CardNumber,CVC,ExpiryDate,Balance,OrderTotal")] CardPayment cardpayment)
            {
      
            if (id != cardpayment.Id)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                            _orderRepository.Update(cardpayment);
                            await _orderRepository.SaveChangesAsync();
                }
            return RedirectToAction("Index", "ShoppingCart");
            }
        }
    }

