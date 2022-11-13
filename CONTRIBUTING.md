# Contributing to Mica For Everyone
PRs are welcome. Use [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) for commit messages. Try to follow the code style of the project.
Also add your name to [MicaForEveryone.UI/SettingsView.xaml.cs#L18](MicaForEveryone.UI/SettingsView.xaml.cs#L18) to get in the list of contributors.

## Translating 
* Fork the repository.

* Translate [this](MicaForEveryone.UI/Strings/en/Resources.resw) file to your language.

* Create a folder with your language code as its name in the [Strings](MicaForEveryone.UI/Strings/) folder and put the translated file in it.

* Add your name to the list of translators in [MicaForEveryone.UI/SettingsView.xaml.cs#L18](MicaForEveryone.UI/SettingsView.xaml.cs#L18) like this:
```cs
internal Contributor[] Translators { get; } = 
{
    ...
    new Contributor("Your Name", "Your GitHub Profile Link", "Your Language Code"),
}
```

* Open a PR. 
