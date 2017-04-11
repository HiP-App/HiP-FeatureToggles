// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Clients.Models
{
    using System.Linq;

    public partial class EntityResult
    {
        /// <summary>
        /// Initializes a new instance of the EntityResult class.
        /// </summary>
        public EntityResult() { }

        /// <summary>
        /// Initializes a new instance of the EntityResult class.
        /// </summary>
        public EntityResult(bool? success = default(bool?), object value = default(object), string errorMessage = default(string))
        {
            Success = success;
            Value = value;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "success")]
        public bool? Success { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "errorMessage")]
        public string ErrorMessage { get; set; }

    }
}