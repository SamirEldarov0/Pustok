namespace Pustok2.Models
{
    public class OrderItem
	{
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? BookId { get; set; }//Book siline biler,eger IsDeleted yazaceyikse,Author,Publisher-leride silmemeliyem cunki
         //oda gedip bookda PubId,AuthorId olan kitablari silecek,kitab silinse buradaki OrderItem-lerde silinecek
        public string Name { get; set; }//Name-ide deyise biler onun ucun name-de burda saxlayiq,amma name eyisme ehtmlai azdi
        public double Price { get; set; }//Price-i bookdan deyismirem cunki bookda qiymeti deyise biler
        public int Count { get; set; }
        public Order Order { get; set; }
        public Book Book { get; set; }
    }
}
