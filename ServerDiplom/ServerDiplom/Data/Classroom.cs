namespace ServerDiplom
{
    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int IdKafedra { get; set; }
        public List<Item> Items { get; set; } // Список имущества в кабинете
    }
}
