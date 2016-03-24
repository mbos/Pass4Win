// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="GitHandling.cs">
//   
// </copyright>
// <summary>
//   Class to interface with the Git Repo
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Pass4Win
{
    using System;
    using System.Linq;
    using System.Net.Sockets;

    using LibGit2Sharp;
    using System.IO;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    /// <summary>
    ///     Class to interface with the Git Repo
    /// </summary>
    public class GitHandling : IDisposable
    {
        // logging
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     The repo remote location object.
        /// </summary>
        private Repository gitRepo;

        /// <summary>
        ///     The host.
        /// </summary>
        private string host;

        /// <summary>
        ///     The repo location.
        /// </summary>
        private readonly string repoLocation;

        /// <summary>
        ///     External or internal GIT.
        ///     When filled in it's external
        /// </summary>
        private readonly string ExternalGitPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHandling"/> class.
        /// </summary>
        /// <param name="repoLocation">
        /// The repo location.
        /// </param>
        /// <param name="host">
        /// The host.
        /// </param>
        public GitHandling(string repoLocation, string host, string GitLocation = "null")
        {
            log.Debug("Init GitHandling");
            this.repoLocation = repoLocation;
            this.ExternalGitPath = GitLocation;
            if (GitLocation == "null")
                this.host = host;
            else
                GetHost();

        }

        /// <summary>
        ///     The dispose function of this class.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool GetHost()
        {
            log.Debug("Getting host from external git executable");
            string result = ExecuteGitCommand("remote -v");
            Regex linkParser = new Regex(@"\b(?:https?://|git)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match m in linkParser.Matches(result))
                this.host = m.Value;
            return true;
        }

        /// <summary>
        /// Checks if repo is valid.
        /// </summary>
        /// <param name="location">
        /// The location.
        /// </param>
        /// <returns>
        /// Sucess <see cref="bool"/>.
        /// </returns>
        public static bool IsValid(string location)
        {
            log.Debug("Check if repo is valid");
            return Repository.IsValid(location);
        }

        /// <summary>
        /// The check connection.
        /// </summary>
        /// <param name="hostName">
        /// The host name.
        /// </param>
        /// <returns>
        /// Succes or failure <see cref="bool"/>.
        /// </returns>
        public static bool CheckConnection(string hostName)
        {
            if (String.IsNullOrEmpty(hostName))
            {
                log.Debug("No remote host, connection check auto passed");
                return true;
            }

            log.Debug("Check if we have a connection to the remote host");
            Uri hostTest;

            if (Uri.TryCreate(hostName, UriKind.Absolute, out hostTest))
            {
                var client = new TcpClient();
                var client2 = new TcpClient();
                var client3 = new TcpClient();
                try
                {
                    var gitAlive = true;
                    var httpsAlive = true;
                    var SSHAlive = true;

                    // Check Git connection
                    var result = client.BeginConnect(hostTest.Authority, 9418, null, null);
                    result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                    if (!client.Connected)
                    {
                        client.Close();
                        gitAlive = false;
                    }

                    // Check HTTPS
                    var result2 = client2.BeginConnect(hostTest.Authority, 443, null, null);
                    result2.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                    if (!client2.Connected)
                    {
                        client2.Close();
                        httpsAlive = false;
                    }

                    // Check SSH
                    var result3 = client3.BeginConnect(hostTest.Authority, 22, null, null);
                    result3.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                    if (!client3.Connected)
                    {
                        client3.Close();
                        SSHAlive = false;
                    }

                    if (gitAlive || httpsAlive || SSHAlive)
                    {
                        return true;
                    }
                }
                catch (Exception message)
                {
                    log.Debug(message);
                    return false;
                }
            }

            // fail
            return false;
        }

        /// <summary>
        ///     The connect to repo.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool ConnectToRepo()
        {
            log.Debug("Connect to repo");
            if (CheckConnection(this.host) && Repository.IsValid(this.repoLocation))
            {
                if (File.Exists(ExternalGitPath))
                {
                    return true;
                }
                else
                {
                    try
                    {
                        this.gitRepo = new Repository(this.repoLocation);

                        // Making sure core.autocrlf = true
                        this.gitRepo.Config.Set("core.autocrlf", true);
                        return true;
                    }
                    catch (Exception message)
                    {
                        log.Debug(message);
                        return false;
                    }
                }
            }
            return false;
        }

        private string ExecuteGitCommand(string command)
        {
            if (!File.Exists(this.ExternalGitPath))
            {
                log.Debug("Git executable not found!");
                return ("fatal");
            }
            log.Debug("Executing: " + command);
            ProcessStartInfo gitInfo = new ProcessStartInfo();
            string output = string.Empty;
            string error = string.Empty;

            gitInfo.CreateNoWindow = true;
            gitInfo.RedirectStandardError = true;
            gitInfo.RedirectStandardOutput = true;
            gitInfo.UseShellExecute = false;
            gitInfo.FileName = this.ExternalGitPath;

            Process gitProcess = new Process();
            gitInfo.Arguments = command;
            gitInfo.WorkingDirectory = this.repoLocation;

            gitProcess.StartInfo = gitInfo;
            gitProcess.Start();

            using (System.IO.StreamReader myOutput = gitProcess.StandardOutput)
            {
                output = myOutput.ReadToEnd();
            }
            using (System.IO.StreamReader myError = gitProcess.StandardError)
            {
                error = myError.ReadToEnd();

            }

            gitProcess.WaitForExit();
            gitProcess.Close();

            // give the output or the error message
            log.Debug("Git output: " + output);
            log.Debug("Git error: " + error);
            if (error.Length == 0) return output;
            else return error;
        }

        /// <summary>
        /// The git clone.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// return boolean <see cref="bool"/>.
        /// </returns>
        public bool GitClone(string username, string password)
        {
            log.Debug("Clone the remote repo");
            if (File.Exists(ExternalGitPath))
            {
                // we don't do this if you have an external git program
            }
            else
            {
                try
                {
                    Repository.Clone(
                        this.host,
                        this.repoLocation,
                        new CloneOptions
                        {
                            CredentialsProvider =
                                    (url, user, cred) =>
                                    new UsernamePasswordCredentials
                                    {
                                        Username = username,
                                        Password = password
                                    }
                        });
                }
                catch (Exception message)
                {
                    log.Debug(message);
                    return false;
                }
            }
            ConnectToRepo();
            return true;
        }

        /// <summary>
        /// Get's the latest and greatest from remote
        /// </summary>
        /// <param name="gitUser">
        /// The Git User.
        /// </param>
        /// <param name="gitPass">
        /// The Git Pass.
        /// </param>
        /// <returns>
        /// A boolean that signals success
        /// </returns>
        public bool Fetch(string gitUser, string gitPass)
        {
            log.Debug("Fetch on remote repo");
            if (File.Exists(ExternalGitPath))
            {
                string GitOutput = ExecuteGitCommand("pull");
                bool result = Regex.IsMatch(GitOutput, "\\bfatal\\b", RegexOptions.IgnoreCase);
                if (result == true)
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    var signature = new Signature(
                        "pass4win",
                        "pull@pass4win.com",
                        new DateTimeOffset(2011, 06, 16, 10, 58, 27, TimeSpan.FromHours(2)));
                    var fetchOptions = new FetchOptions
                    {
                        CredentialsProvider =
                                                   (url, user, cred) =>
                                                   new UsernamePasswordCredentials
                                                   {
                                                       Username = gitUser,
                                                       Password = gitPass
                                                   }
                    };
                    var mergeOptions = new MergeOptions();
                    var pullOptions = new PullOptions { FetchOptions = fetchOptions, MergeOptions = mergeOptions };
                    this.gitRepo.Network.Pull(signature, pullOptions);
                }
                catch (Exception message)
                {
                    log.Debug(message);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// The commit.
        /// </summary>
        /// <param name="fileToCommit">
        /// The file to commit.
        /// </param>
        /// <returns>
        /// Result in boolean <see cref="bool"/>.
        /// </returns>
        public bool Commit(string fileToCommit = null)
        {
            log.Debug("Commit: " + fileToCommit);
            // Stage the file
            if (fileToCommit != null)
            {
                if (File.Exists(ExternalGitPath))
                {
                    string GitOutput = ExecuteGitCommand("add " + fileToCommit);
                    bool result = Regex.IsMatch(GitOutput, "\\bfatal\\b", RegexOptions.IgnoreCase);
                    if (result == true)
                    {
                        return false;
                    }
                    GitOutput = ExecuteGitCommand("commit -m Pass4Win");
                    result = Regex.IsMatch(GitOutput, "\\bfatal\\b", RegexOptions.IgnoreCase);
                    if (result == true)
                    {
                        return false;
                    }
                }
                else
                {
                    RepositoryStatus status = gitRepo.RetrieveStatus();
                    if (status.IsDirty)
                    {
                        try
                        {
                            gitRepo.Stage("*");
                            gitRepo.Commit(
                            "password changes",
                            new Signature("pass4win", "pass4win", DateTimeOffset.Now),
                            new Signature("pass4win", "pass4win", DateTimeOffset.Now));
                        }
                        catch (Exception message)
                        {
                            log.Debug(message);
                            return false;
                        }
                    }
                }
            }
            // Commit
            return true;
        }

        /// <summary>
        /// Push function.
        /// </summary>
        /// <param name="gitUser">
        /// The git user.
        /// </param>
        /// <param name="gitPass">
        /// The git pass.
        /// </param>
        /// <returns>
        /// Result in boolean <see cref="bool"/>.
        /// </returns>
        public bool Push(string gitUser, string gitPass)
        {
            log.Debug("Push commits");
            if (File.Exists(ExternalGitPath))
            {
                string GitOutput = ExecuteGitCommand("push");
                bool result = Regex.IsMatch(GitOutput, "\\bfatal\\b", RegexOptions.IgnoreCase);
                if (result == true)
                {
                    return false;
                }
            }
            else
            {
                var tc = this.gitRepo.Diff.Compare<TreeChanges>(this.gitRepo.Branches["origin/master"].Tip.Tree, this.gitRepo.Head.Tip.Tree);
                if (!tc.Any())
                {
                    return true;
                }
                var remote = this.gitRepo.Network.Remotes["origin"];
                var options = new PushOptions
                {
                    CredentialsProvider =
                                          (url, user, cred) =>
                                          new UsernamePasswordCredentials
                                          {
                                              Username = gitUser,
                                              Password = gitPass
                                          }
                };
                var pushRefSpec = @"refs/heads/master";
                try
                {
                    this.gitRepo.Network.Push(remote, pushRefSpec, options);
                }
                catch (Exception message)
                {
                    log.Debug(message);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Removes a file from the Git repo
        /// </summary>
        /// <param name="removeFile">
        /// The filename.
        /// </param>
        /// <returns>
        /// result in boolean<see cref="bool"/>.
        /// </returns>
        public bool RemoveFile(string removeFile)
        {
            log.Debug("Remove: " + removeFile);
            if (File.Exists(ExternalGitPath))
            {
                string GitOutput = ExecuteGitCommand("rm " + removeFile);
                bool result = Regex.IsMatch(GitOutput, "\\bfatal\\b", RegexOptions.IgnoreCase);
                if (result == true)
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    this.gitRepo.Remove(removeFile);
                }
                catch (Exception message)
                {
                    log.Debug(message);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Free the resources.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.gitRepo.Dispose();
            }
        }
    }
}