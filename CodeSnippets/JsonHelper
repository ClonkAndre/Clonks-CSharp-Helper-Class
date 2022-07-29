/// <summary>
/// Newtonsoft.Json helper things.
/// </summary>
public class JsonHelper {

    /// <summary>
    /// Helper class for Newtonsoft.Json Library to ignore properties from serialization.
    /// </summary>
    public class IgnorePropertiesResolver : Newtonsoft.Json.Serialization.DefaultContractResolver {

        #region Variables
        private readonly HashSet<string> ignoreProps;
        #endregion

        #region Constructor
        public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore)
        {
            ignoreProps = new HashSet<string>(propNamesToIgnore);
        }
        #endregion

        protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            Newtonsoft.Json.Serialization.JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (ignoreProps.Contains(property.PropertyName)) {
                property.ShouldSerialize = _ => false;
            }
            return property;
        }

    }

}
