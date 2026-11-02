namespace GYM_Desktop_app.Models
{
    // One sellable item. Each size is its own product, with its own price and stock.
    public class Product
    {
        // The categories offered on the Products page. Edit this list to add more.
        public static readonly string[] Categories =
        {
            "Boxing/MMA Gloves", "Jiu-Jitsu Gi", "T-Shirt", "Shorts",
            "Mouthguard", "Shin Guards", "Bandages"
        };

        public int ProductID { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
