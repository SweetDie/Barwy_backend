namespace Barwy.Data.Data.Managers
{
    public class ProductManagerResult
    {
        public ProductManagerResult()
        {
            Errors= new List<string>();
        }

        public  List<string> Errors { get; set; }
        public bool Succeeded { get; set; }
    }
}
