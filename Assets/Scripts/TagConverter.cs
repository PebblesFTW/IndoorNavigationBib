public class TagConverter {

    public static string getTagName(int enumInt) {
        switch (enumInt) {
            case ((int) EnumTags.Books):
                return "Buch";
            case ((int) EnumTags.Norms):
                return "Normen";
            case ((int) EnumTags.Theses):
                return "Abschlussarbeiten";
            case ((int) EnumTags.NewBooks):
                return "Neuerwerbungen";
            case ((int) EnumTags.BookSell):
                return "Buchverkauf";
            case ((int) EnumTags.LooseLeafCollection):
                return "Loseblattsammlung";
            case ((int) EnumTags.MagazineCurrent):
                return "ZeitschriftenAktuell";
            case ((int) EnumTags.MagazineOld):
                return "ZeitschriftenAlt";
            case ((int) EnumTags.MagazineSingle):
                return "ZeitschriftenEinzelhefte";
            default:
                return null;
        }
    }

    public enum EnumTags {
        Books = 0,
        Norms = 1,
        Theses = 2,
        NewBooks = 3,
        BookSell = 4,
        LooseLeafCollection = 5,
        MagazineCurrent = 6,
        MagazineOld = 7,
        MagazineSingle = 8
    }
}
