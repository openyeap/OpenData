using System;
using System.IO;
using System.Text;

namespace OpenData.Utility
{


    public class JSMinify
    {
        const int EOF = -1;

        private TextReader sr;
        private TextWriter sw;
        private int theA;
        private int theB;
        private int theLookahead = EOF;

        public static string Minify(string js)
        {
            JSMinify jsMinify = new JSMinify();
            StringBuilder sb = new StringBuilder();

            using (jsMinify.sr = new StringReader(js))
            {
                using (jsMinify.sw = new StringWriter(sb))
                {
                    jsMinify.jsmin();
                }
            }

            return sb.ToString();
        }

        private JSMinify() { }

        /* jsmin -- Copy the input to the output, deleting the characters which are
                insignificant to JavaScript. Comments will be removed. Tabs will be
                replaced with spaces. Carriage returns will be replaced with linefeeds.
                Most spaces and linefeeds will be removed.
        */
        void jsmin()
        {
            theA = '\n';
            action(3);
            while (theA != EOF)
            {
                switch (theA)
                {
                    case ' ':
                        {
                            if (isAlphanum(theB))
                            {
                                action(1);
                            }
                            else
                            {
                                action(2);
                            }
                            break;
                        }
                    case '\n':
                        {
                            switch (theB)
                            {
                                case '{':
                                case '[':
                                case '(':
                                case '+':
                                case '-':
                                    {
                                        action(1);
                                        break;
                                    }
                                case ' ':
                                    {
                                        action(3);
                                        break;
                                    }
                                default:
                                    {
                                        if (isAlphanum(theB))
                                        {
                                            action(1);
                                        }
                                        else
                                        {
                                            action(2);
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            switch (theB)
                            {
                                case ' ':
                                    {
                                        if (isAlphanum(theA))
                                        {
                                            action(1);
                                            break;
                                        }
                                        action(3);
                                        break;
                                    }
                                case '\n':
                                    {
                                        switch (theA)
                                        {
                                            case '}':
                                            case ']':
                                            case ')':
                                            case '+':
                                            case '-':
                                            case '"':
                                            case '\'':
                                                {
                                                    action(1);
                                                    break;
                                                }
                                            default:
                                                {
                                                    if (isAlphanum(theA))
                                                    {
                                                        action(1);
                                                    }
                                                    else
                                                    {
                                                        action(3);
                                                    }
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        action(1);
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
        }
        /* action -- do something! What you do is determined by the argument:
                1   Output A. Copy B to A. Get the next B.
                2   Copy B to A. Get the next B. (Delete A).
                3   Get the next B. (Delete B).
           action treats a string as a single character. Wow!
           action recognizes a regular expression if it is preceded by ( or , or =.
        */
        void action(int d)
        {
            if (d <= 1)
            {
                put(theA);
            }
            if (d <= 2)
            {
                theA = theB;
                if (theA == '\'' || theA == '"')
                {
                    for (; ; )
                    {
                        put(theA);
                        theA = get();
                        if (theA == theB)
                        {
                            break;
                        }
                        if (theA <= '\n')
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}\n", theA));
                        }
                        if (theA == '\\')
                        {
                            put(theA);
                            theA = get();
                        }
                    }
                }
            }
            if (d <= 3)
            {
                theB = next();
                if (theB == '/' && (theA == '(' || theA == ',' || theA == '=' ||
                                    theA == '[' || theA == '!' || theA == ':' ||
                                    theA == '&' || theA == '|' || theA == '?' ||
                                    theA == '{' || theA == '}' || theA == ';' ||
                                    theA == '\n'))
                {
                    put(theA);
                    put(theB);
                    for (; ; )
                    {
                        theA = get();
                        if (theA == '/')
                        {
                            break;
                        }
                        else if (theA == '\\')
                        {
                            put(theA);
                            theA = get();
                        }
                        else if (theA <= '\n')
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.\n", theA));
                        }
                        put(theA);
                    }
                    theB = next();
                }
            }
        }
        /* next -- get the next character, excluding comments. peek() is used to see
                if a '/' is followed by a '/' or '*'.
        */
        int next()
        {
            int c = get();
            if (c == '/')
            {
                switch (peek())
                {
                    case '/':
                        {
                            for (; ; )
                            {
                                c = get();
                                if (c <= '\n')
                                {
                                    return c;
                                }
                            }
                        }
                    case '*':
                        {
                            get();
                            for (; ; )
                            {
                                switch (get())
                                {
                                    case '*':
                                        {
                                            if (peek() == '/')
                                            {
                                                get();
                                                return ' ';
                                            }
                                            break;
                                        }
                                    case EOF:
                                        {
                                            throw new Exception("Error: JSMIN Unterminated comment.\n");
                                        }
                                }
                            }
                        }
                    default:
                        {
                            return c;
                        }
                }
            }
            return c;
        }
        /* peek -- get the next character without getting it.
        */
        int peek()
        {
            theLookahead = get();
            return theLookahead;
        }
        /* get -- return the next character from stdin. Watch out for lookahead. If
                the character is a control character, translate it to a space or
                linefeed.
        */
        int get()
        {
            int c = theLookahead;
            theLookahead = EOF;
            if (c == EOF)
            {
                c = sr.Read();
            }
            if (c >= ' ' || c == '\n' || c == EOF)
            {
                return c;
            }
            if (c == '\r')
            {
                return '\n';
            }
            return ' ';
        }
        void put(int c)
        {
            sw.Write((char)c);
        }
        /* isAlphanum -- return true if the character is a letter, digit, underscore,
                dollar sign, or non-ASCII character.
        */
        bool isAlphanum(int c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
                c > 126 || c == '+' || c == '-');
        }
    }

    ///<exclude/>
    public sealed class JavaScriptMinifier : IDisposable
    {
        const int EOF = -1;

        StreamReader sr;
        StreamWriter sw;
        StringBuilder sb = new StringBuilder();
        MemoryStream msSrc;
        int theA;
        int theB;
        int theLookahead = EOF;
        bool isFileMode;

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("invalid arguments, 2 required, 1 in, 1 out");
                return;
            }
            new JavaScriptMinifier().Minify(args[0], args[1]);
        }

        public string Minify(string src)
        {
            return Minify(src, Encoding.UTF8);
        }

        public string Minify(string src, Encoding encoding)
        {
            isFileMode = false;
            msSrc = new MemoryStream(encoding.GetBytes(src));
            using (sr = new StreamReader(msSrc))
            {
                jsmin();
            }

            return sb.ToString();
        }

        public string Minify(string src, string dst)
        {
            isFileMode = true;
            using (sr = new StreamReader(src))
            {
                using (sw = new StreamWriter(dst))
                {
                    jsmin();
                }
            }

            return sb.ToString();
        }

        /* jsmin -- Copy the input to the output, deleting the characters which are
                insignificant to JavaScript. Comments will be removed. Tabs will be
                replaced with spaces. Carriage returns will be replaced with linefeeds.
                Most spaces and linefeeds will be removed.
        */
        void jsmin()
        {
            theA = '\n';
            action(3);
            while (theA != EOF)
            {
                switch (theA)
                {
                    case ' ':
                        {
                            if (isAlphanum(theB))
                            {
                                action(1);
                            }
                            else
                            {
                                action(2);
                            }
                            break;
                        }
                    case '\n':
                        {
                            switch (theB)
                            {
                                case '{':
                                case '[':
                                case '(':
                                case '+':
                                case '-':
                                    {
                                        action(1);
                                        break;
                                    }
                                case ' ':
                                    {
                                        action(3);
                                        break;
                                    }
                                default:
                                    {
                                        if (isAlphanum(theB))
                                        {
                                            action(1);
                                        }
                                        else
                                        {
                                            action(2);
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            switch (theB)
                            {
                                case ' ':
                                    {
                                        if (isAlphanum(theA))
                                        {
                                            action(1);
                                            break;
                                        }
                                        action(3);
                                        break;
                                    }
                                case '\n':
                                    {
                                        switch (theA)
                                        {
                                            case '}':
                                            case ']':
                                            case ')':
                                            case '+':
                                            case '-':
                                            case '"':
                                            case '\'':
                                                {
                                                    action(1);
                                                    break;
                                                }
                                            default:
                                                {
                                                    if (isAlphanum(theA))
                                                    {
                                                        action(1);
                                                    }
                                                    else
                                                    {
                                                        action(3);
                                                    }
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        action(1);
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
        }
        /* action -- do something! What you do is determined by the argument:
                1   Output A. Copy B to A. Get the next B.
                2   Copy B to A. Get the next B. (Delete A).
                3   Get the next B. (Delete B).
           action treats a string as a single character. Wow!
           action recognizes a regular expression if it is preceded by ( or , or =.
        */
        void action(int d)
        {
            if (d <= 1)
            {
                put(theA);
            }
            if (d <= 2)
            {
                theA = theB;
                if (theA == '\'' || theA == '"')
                {
                    for (; ; )
                    {
                        put(theA);
                        theA = get();
                        if (theA == theB)
                        {
                            break;
                        }
                        if (theA <= '\n')
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}\n", theA));
                        }
                        if (theA == '\\')
                        {
                            put(theA);
                            theA = get();
                        }
                    }
                }
            }
            if (d <= 3)
            {
                theB = next();
                if (theB == '/' && (theA == '(' || theA == ',' || theA == '=' ||
                    theA == '[' || theA == '!' || theA == ':' ||
                    theA == '&' || theA == '|' || theA == '?' ||
                    theA == '{' || theA == '}' || theA == ';' ||
                    theA == '\n'))
                {
                    put(theA);
                    put(theB);
                    for (; ; )
                    {
                        theA = get();
                        if (theA == '/')
                        {
                            break;
                        }
                        else if (theA == '\\')
                        {
                            put(theA);
                            theA = get();
                        }
                        else if (theA <= '\n')
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.\n", theA));
                        }
                        put(theA);
                    }
                    theB = next();
                }
            }
        }
        /* next -- get the next character, excluding comments. peek() is used to see
                if a '/' is followed by a '/' or '*'.
        */
        int next()
        {
            int c = get();
            if (c == '/')
            {
                switch (peek())
                {
                    case '/':
                        {
                            for (; ; )
                            {
                                c = get();
                                if (c <= '\n')
                                {
                                    return c;
                                }
                            }
                        }
                    case '*':
                        {
                            get();
                            for (; ; )
                            {
                                switch (get())
                                {
                                    case '*':
                                        {
                                            if (peek() == '/')
                                            {
                                                get();
                                                return ' ';
                                            }
                                            break;
                                        }
                                    case EOF:
                                        {
                                            throw new Exception("Error: JSMIN Unterminated comment.\n");
                                        }
                                }
                            }
                        }
                    default:
                        {
                            return c;
                        }
                }
            }
            return c;
        }
        /* peek -- get the next character without getting it.
        */
        int peek()
        {
            theLookahead = get();
            return theLookahead;
        }
        /* get -- return the next character from stdin. Watch out for lookahead. If
                the character is a control character, translate it to a space or
                linefeed.
        */
        int get()
        {
            int c = theLookahead;
            theLookahead = EOF;
            if (c == EOF)
            {
                c = sr.Read();
            }
            if (c >= ' ' || c == '\n' || c == EOF)
            {
                return c;
            }
            if (c == '\r')
            {
                return '\n';
            }
            return ' ';
        }
        void put(int c)
        {
            if (this.isFileMode)
                sw.Write((char)c);
            else
                sb.Append((char)c);
        }
        /* isAlphanum -- return true if the character is a letter, digit, underscore,
                dollar sign, or non-ASCII character.
        */
        bool isAlphanum(int c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
                c > 126);
        }

        public void Dispose()
        {
            msSrc = null;
        }
    }

}