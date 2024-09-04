namespace PlayCodeApi.Contract.V1;

public static class ApiRoutes
{
    public const string Base = "v1";
    
    public static class PlayCodes
    {
        public const string Get = Base + "/playcodes/{code}";
        public const string Purchase = Base + "/playcodes/purchase";
        public const string TopUp = Base + "/playcodes/{code}/topup";
        public const string CashOut = Base + "/playcodes/{code}/cashout";
    }
}