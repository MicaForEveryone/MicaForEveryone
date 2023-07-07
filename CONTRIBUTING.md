# Contributing
PRs are welcome, but please keep these notes in mind:
* If possible, use [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) for commit messages.
* Try to follow the code style of the project.
* Add your name to [MicaForEveryone.UI/SettingsView.xaml.cs#L18](MicaForEveryone.UI/SettingsView.xaml.cs#L18) to get in the list of contributors.

## Translating 
* Fork the repository.

* Translate [this](MicaForEveryone.UI/Strings/en/Resources.resw) file to your language.

* Create a folder with your [BCP-47 language tag](https://www.iana.org/assignments/language-subtag-registry/language-subtag-registry) as its name in the [Strings](MicaForEveryone.UI/Strings/) folder and put the translated file in it.

* If you are not using Visual Studio, add the new file to the [project file](MicaForEveryone.UI/MicaForEveryone.UI.csproj#201) manually like this:
```xml
<PRIResource Include="Strings\<Your Language Tag>\Resources.resw">
    <SubType>Designer</SubType>
</PRIResource>
```

* Add your name to the list of translators in [MicaForEveryone.UI/SettingsView.xaml.cs#L18](MicaForEveryone.UI/SettingsView.xaml.cs#L18) like this:
```cs
internal Contributor[] Translators { get; } = 
{
    ...
    new Contributor("Your Name", "Your GitHub Profile Link", "Your Language Code"),
}
```

* Open a PR. 
