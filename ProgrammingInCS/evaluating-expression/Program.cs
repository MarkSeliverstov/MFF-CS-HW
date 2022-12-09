namespace EvalExpression
{
    class Tree{

    }

    struct Node{
        public int? value;
        public char? op;
        public Node(int? value, char? op){
            this.value = value;
            this.op = op;
        }
    }

    class Program{
        static void Main(string[] args){
            // string expression = "+ ~ 1 3";
            // string expression = "/ + - 5 2 * 2 + 3 3 ~ 2";
            // string expression = "- - 2000000000 2100000000 2100000000";
            // string expression = "+ 1 2 3";
            // string expression = "- 2000000000 4000000000";
            // string expression = "1+2*3";
            string expression = "1+2*3";
            int result = EvalExpression(expression);
            Console.WriteLine(result);
        }
        
    }
}