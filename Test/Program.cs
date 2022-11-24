// See https://aka.ms/new-console-template for more information
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.VirastyarInlineVerifiers;
static string GetFullPath(string dataFileName)
{
    string path = AppDomain.CurrentDomain.BaseDirectory;
    return path + "\\dic\\" + dataFileName;
}

var engine = new PersianSpellChecker(new PersianSpellCheckerConfig()
{
    DicPath = GetFullPath("Dic.dat"),
    StemPath = GetFullPath("Stem.dat"),
    EditDistance = 1,
    SuggestionCount = 1
});
var Verifier_Main = new SpellCheckerInlineVerifier(false, engine);
var sampleWord = "زرری";
Console.WriteLine("Suggestion count for:" + sampleWord);
Console.WriteLine(Verifier_Main.VerifyParagraph("زرری").FirstOrDefault()?.Suggestions?.Length);
Console.ReadLine();