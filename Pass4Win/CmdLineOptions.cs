using CommandLine;

namespace Pass4Win
{
    class CmdLineOptions
    {
        [Option('d', "debug", DefaultValue = false, HelpText = "Set the log file to your personal directory")]
        public bool Debug { get; set; }

        [Option('r', "reset", DefaultValue = false, HelpText = "Resets the config file")]
        public bool Reset { get; set; }

        [Option('g', "nogit", DefaultValue = false, HelpText = "Use git.")]
        public bool NoGit { get; set; }

    }
}