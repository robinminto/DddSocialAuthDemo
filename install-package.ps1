gci -recurse -filter packages.config | %{ nuget.exe install $_.fullname -outputdirectory Packages }