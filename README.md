# Pass4Win
Windows version of Pass (http://www.passwordstore.org/) in the sense that the store (password structure) is and should be exactly the same between the two programs.

This is **alpha software**. The following functionality is not implemented yet:
- Adding passwords
- Git integration

I really liked the idea of a password store I could access on my shell, and even Android devices. But my main PC is a Windows machine.
So I needed a Windows implementation.....

Make sure you have GnuPG for windows installed (http://www.gpg4win.org/) and have at least one secret key.
You need to compile it yourself for the moment as I'm not going to provide binaries for an alpha release.

I'm using GpgAPI (https://gpgapi.codeplex.com/) for interfacing with GnuPG. I used nupkg, but you can also build it yourself.


Regards,

Mike
