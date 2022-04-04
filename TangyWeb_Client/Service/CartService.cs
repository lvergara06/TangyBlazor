using Blazored.LocalStorage;
using TangyWeb_Client.Service.IService;
using Tangy_Common;

namespace TangyWeb_Client.Service
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorageService;

        public event Action OnChange;

        public CartService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }
        public async Task IncrementCart(ShoppingCart ShoppingCart)
        {
            var newCart = await _localStorageService.GetItemAsync<List<ShoppingCart>>(SD.ShoppingCart);
            bool itemInCart = false; 

            if (newCart ==null)
            {
                newCart = new List<ShoppingCart>();
            }

            foreach(var obj in newCart)
            {
                if(obj.ProductPriceId == ShoppingCart.ProductPriceId && obj.ProductId == ShoppingCart.ProductId)
                {
                    itemInCart = true;
                    obj.Count += ShoppingCart.Count;
                }
            }

            if(!itemInCart)
            {
                newCart.Add(new ShoppingCart()
                {
                    ProductId = ShoppingCart.ProductId,
                    Product = ShoppingCart.Product,
                    ProductPriceId = ShoppingCart.ProductPriceId,
                    ProductPrice = ShoppingCart.ProductPrice,
                    Count = ShoppingCart.Count
                });
            }

            await _localStorageService.SetItemAsync(SD.ShoppingCart, newCart);
            OnChange.Invoke();

        }

        public async Task DecrementCart(ShoppingCart ShoppingCart)
        {
            var newCart = await _localStorageService.GetItemAsync<List<ShoppingCart>>(SD.ShoppingCart);

            for(int i = 0; i < newCart.Count; i++)
            {
                if (newCart[i].ProductPriceId == ShoppingCart.ProductPriceId && newCart[i].ProductId == ShoppingCart.ProductId)
                {
                    if(newCart[i].Count == 1 || ShoppingCart.Count == 0)
                    {
                        newCart.RemoveAt(i);
                    }
                    else
                    {
                        newCart[i].Count -= 1;
                    }
                }
            }

            await _localStorageService.SetItemAsync(SD.ShoppingCart, newCart);
            OnChange.Invoke();
        }
    }
}
