using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class NewString
    {
        public int id { get; private set; }
        public string value { get; private set; }

        public NewString(int id, string value)
        {
            this.id = id;
            this.value = value;
        }
    }
}
