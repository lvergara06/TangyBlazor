namespace TangyWeb_Client.Service.IService
{
    public interface ICartService
    {
        public event Action OnChange;
        Task IncrementCart(ShoppingCart ShoppingCart);
        Task DecrementCart(ShoppingCart ShoppingCart);
    }
}
