namespace PlayCodeApi.Contract.V1;

public static class ApiRoutes
{
    public const string Base = "v1";
    
    public static class PlayCodes
    {
        public const string PlayCodeBase = Base + "/system/{systemId}/location/{locationId}";
        public const string Get = PlayCodeBase + "/playcodes/{code}";
        public const string Purchase = PlayCodeBase + "/playcodes/purchase";
        public const string TopUp = PlayCodeBase + "/playcodes/{code}/topup";
        public const string CashOut = PlayCodeBase + "/playcodes/{code}/cashout";
    }
}