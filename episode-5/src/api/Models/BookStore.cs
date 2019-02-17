using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreApp.WebApi.Models
{
    /// <summary>
    /// BookStore POCO.
    /// </summary>
    public class BookStore
    {
        [BsonId]
        public ObjectId  Id {get; set;}
        public string Name {get;set;}
        public string AddressLine1 {get;set;}
        public string AddressLine2 {get;set;}
        public string AddressLine3 {get;set;}
        public string City {get;set;}
        public string PostCode {get;set;}
        public string TelephoneNumber {get;set;}

        /// <summary>
        /// Default constructor
        /// </summary>
        public BookStore()
        {
            this.Id = ObjectId.GenerateNewId();
        }
    }
}