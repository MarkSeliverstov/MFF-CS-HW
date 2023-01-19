using Xunit;
namespace semestr_test
{
	public class TestParser
	{
		[Fact]
		public void TestParseDelimiter1()
        {
            Assert.Equal("|", Parser.ParseDelimiter("\"|\""));
        }
        [Fact]
        public void TestParseDelimiter2()
        {
            Assert.Equal(" | ", Parser.ParseDelimiter("\" | \""));
        }

        [Fact]
        public void TestParse1_delimiter_PasteFile(){
            string inputfile = "data/file.in.txt";
            string delimiter = " ";
            List<int> fields = null;
            string outputDelimiter = "|";
            ActionsWithFile? actions = new ActionsWithFile(inputfile, delimiter, fields);

            string input = "--od \"|\" -d \" \" data/file.in.txt";

            string[] args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            CutAndPaste? actual = Parser.Parse(args);

            CutAndPaste? expected = new CutAndPaste(
                new List<ActionsWithFile>(){
                    actions
                }, null, outputDelimiter);
            Assert.Equivalent(expected, actual);
        }
        
	}
}