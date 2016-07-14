using CommandLine;
using CommandLine.Text;


namespace Pass4Win
{
    class CmdLineOptions
    {
        [Option('d', "debug", DefaultValue = false, HelpText = "Set the log file to your personal directory")]
        public bool Debug { get; set; }

        [Option('r', "reset", DefaultValue = false, HelpText = "Resets the config file")]
        public bool Reset { get; set; }

        [Option('g', "git", DefaultValue = true, HelpText = "Use git.")]
        public bool UseGit { get; set; }

    }
}