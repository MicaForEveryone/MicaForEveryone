# Be Iranian's voice!
Right now as you read this, the government is killing hundreds of protestors in Iran.
People are seeking freedom while they have highly restricted access to Internet and only gevornment-based media are allowed to operate.

* In July 1999 they attacked students nightly in dorm because they protested for freedom of newspapers.
* In December 2019 - January 2020 people were protesting for increase of prices and then the government killed more than 1500 of them. We experienced full Internet shutdown during that time.
* In September 2022 the <!--morality--> police killed an innocent girl and caused <!--rise--> a wave of unhappy people, and since then people are fighting. In these days we hear a lot about the arrest of innocent people and protestors, and even <!--the--> people who did nothing.

Jadi Mirmirani, an Iranian open-source activist and Mohsen Tahmasbi, an Iranian security researcher are among the arrested people in October 2022.

# Mica For Everyone!
Mica For Everyone is a tool to customize system backdrop on Win32 apps using [DwmSetWindowAttribute](https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmsetwindowattribute) and other methods.
It can apply Mica (or any other backdrop materials) on the non-client area (window frame) or background of supported apps and its behavior is customizable through a GUI and a config file.

**NOTE**: Mica For Everyone is not responsible for rendering the effects you set, it just asks Windows to do that for you. If there's any problem with the effects it's a third-party issue. Try creating a rule for the affected apps and try different settings before opening an issue for it.

## Config File
For more information check our [wiki page](https://github.com/MicaForEveryone/MicaForEveryone/wiki/Config-File) and [default config file](MicaForEveryone/Resources/MicaForEveryone.conf).

## Contributing
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

## Screenshots

![Screenshot 1](Assets/1.png)

![Screenshot 2](Assets/2.png)

![Screenshot 3](Assets/3.png)

## Frequency Asked Questions
Check [wiki page](https://github.com/MicaForEveryone/MicaForEveryone/wiki/FAQ) or [issues](https://github.com/MicaForEveryone/MicaForEveryone/issues).
