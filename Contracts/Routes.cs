namespace WarehouseManagement.Contracts;

public static class Routes
{
    public static class Balances
    {
        public const string Base = "/balances";
        public const string Api = "/api/balance";
    }

    public static class Receipts
    {
        public const string Base = "/receiptsDocument";
        public const string Api = "/api/receipts";
    }

    public static class Shipments
    {
        public const string Base = "/shipmentsDocument";
        public const string Api = "/api/shipments";
    }

    public static class Resources
    {
        public const string Base = "/resources";
        public const string Api = "/api/resources";
    }

    public static class Clients
    {
        public const string Base = "/clients";
        public const string Api = "/api/clients";
    }

    public static class Units
    {
        public const string Base = "/units";
        public const string Api = "/api/units";
    }

    public static string TransformUrl(string route, int id)
    {
        return route + "/" + id;
    }
}
