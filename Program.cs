using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QnDoptimiser
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Specify the TXT/MML file to 'optimise' on the command line, or drag+drop a TXT/MML file on to me.");
                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
                return;
            }

            string filename = string.Join(" ", args).Replace("\"",""); // Convert args[] to single string, and delete enclosing quotes.

            if (!File.Exists(filename))
            {
                Console.WriteLine(filename + " doesn't exist boofhead. Press a key to exit.");
                Console.ReadKey();
                return;
            }

            string outfilename = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename)) + ".optim.txt";

            try
            {

                var infileContents = File.ReadAllLines(filename).ToList();
                var outputStorage = new StringBuilder();

                List<string> dict = new List<string>();
                dict.Add("//// Reserved by RetroSwim hahahaaa!!! ////"); // Reserve list index 0, cause we can't use that as a loop number. I don't think.

                bool ithasbegun = false;

                for(int i = 0; i < infileContents.Count; i++)
                {
                    if (ithasbegun && !string.IsNullOrEmpty(infileContents[i].Trim())) // Skip empty lines, append as-is
                    {
                        var findIndex = dict.IndexOf(infileContents[i].Trim());
                        if (findIndex > -1)
                        {
                            // Found this line before and have a token for it
                            outputStorage.AppendLine($"({findIndex})1");

                        }
                        else
                        {
                            // Very slowly look ahead and see if this line appears again somewhere down the line
                            if(infileContents.Count(s => s.Trim() == infileContents[i].Trim()) > 1)
                            {
                                // Cool, it is, make this line a loop.
                                dict.Add(infileContents[i].Trim());
                                outputStorage.AppendLine($"({dict.Count - 1})[{infileContents[i].Trim()}]1");
                            }
                            else
                            {
                                // It's a loner. Append it to the output as-is.
                                outputStorage.AppendLine(infileContents[i].Trim());
                            }
                        }
                    }
                    else
                    {
                        outputStorage.AppendLine(infileContents[i].Trim());
                    }
                    // Quick and Dirty safety feature, only start on the line following a line starting with #0.
                    if (infileContents[i].Trim().StartsWith("#0")) ithasbegun = true;

                    

                }

                if (File.Exists(outfilename))
                {
                    Console.WriteLine("Overwriting " + outfilename);
                    try
                    {
                        File.Delete(outfilename);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Couldn't overwrite target file. Maybe this is why --> " + ex.Message);
                        Console.WriteLine("Press a key to exit.");
                        Console.ReadKey();
                        return;
                    }
                    
                }
                File.WriteAllText(outfilename, outputStorage.ToString());

                var fi_in = new FileInfo(filename);
                var fi_out = new FileInfo(outfilename);

                Console.WriteLine($"Created {dict.Count - 1} unique loops, reduced file size from {fi_in.Length} to {fi_out.Length} (saving {fi_in.Length - fi_out.Length} bytes)");
                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
                return;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Something fucked up, probably couldn't open the file you dragged on to me. This might say why --> " + ex.Message);
                Console.WriteLine("Press a key to exit.");
                Console.ReadKey();
            }

        }
    }
}
