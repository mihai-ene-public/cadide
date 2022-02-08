namespace IDE.Documents.Views
{
    public class PartBomOutputItemDisplay : PartBomItemDisplay
    {
        public PartBomOutputItemDisplay() { }
        public PartBomOutputItemDisplay(PartBomItemDisplay bomItem)
        {
            PartName = bomItem.PartName;
            Comment = bomItem.Comment;
            Component = bomItem.Component;
            Supplier = bomItem.Supplier;
            Sku = bomItem.Sku;
            Manufacturer = bomItem.Manufacturer;
            MPN = bomItem.MPN;
            Description = bomItem.Description;
            RoHS = bomItem.RoHS;
            Package = bomItem.Package;
            Packaging = bomItem.Packaging;
            Stock = bomItem.Stock;
            Currency = bomItem.Currency;
            Properties = bomItem.Properties;
            Prices = bomItem.Prices;
        }

        public int Quantity { get; set; }

        public double Price { get; set; }
    }
}
