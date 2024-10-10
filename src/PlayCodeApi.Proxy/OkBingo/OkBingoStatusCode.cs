namespace PlayCodeApi.Proxy.ApiHandler;

public enum OkBingoStatusCode
{
    Success = 0,                            //	OK
    InvalidAmountForPurchase = 101,         //	Kr beløp må være mellom 1 og 1000 kr
    InvalidAmountForTopUp = 201,            //	Kr beløp må være mellom 1 og 1000 kr
    InvalidTicketNumberForTopUp = 203,      //	Gyldig billettnr må oppgis.
    InvalidTicketNumberForCloseTicket = 303,//	Gyldig billettnr må oppgis.
    TicketAlreadyClosed = 404,              //	Ticket er allerede lukket
    ValueGreaterThanLimitInCAS = 501,       //	Verdien er større enn limit parameter i CAS
    ValueCannotBeZero = 502,                //	Verdien kan ikke være 0
    LSNotRegisteredInCAS = 503,             //	LS er ikke registrert i CAS
    PSKeyNotRegistered = 504,               //	PSKey er ikke registrert
    InvalidPSKey = 505,                     //	Feil PSKey	
    InvalidCASDate = 506,                   //	CAS datoen er feil
    TicketNotFound = 507,                   //	Ticket er ikke funnet
    TicketIsClosed = 508,                   //	Ticket er stengt
    TicketIsExpired = 509,                  //	Ticket er utgått
    TicketIsInUse = 510,                    //	Ticket er i bruk / logget inn på maskin
    InvalidTicket = 511,                    //	Ticket er invalid
    OperationFailed = 512,                  //	ERROR i operasjon, prøv igjen
    CASIsClosed = 513,                      //	CAS Penger er stengt
    PSKeyIsBlocked = 514,                   //	PSKey er blokkert!
}