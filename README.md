# Space Invaders

This is a clone of the famous classical arcade game Space Invaders in C# .NET WPF.

## Additional considerations
Into this project, I don't use graphical image files but I generate the required sprites.
I use exclusivelly WPF functions but it can easily adapted in other frameworks because drawing stuff is easily localisable to be updated.
All the code can be quickly moveable in an foreign project (without needing to do a DLL) because I wanted to embed it into another project of mine as an Easter Egg.
The project uses only one timer uses to draw sprites and do the game loop keeping the traditional way of programming games in 80s.

## Getting Started

To be able to compile this project, it is very simple, you only need to download or clone the sources locally on your computer and open the solution from Visual Studio 2017 or higher but you could open it from a lower version of Visual Studio by remaking the solution with your current version.

### Prerequisites

* Microsoft Visual Studio 2017
* .NET Framework 4

## Motivation

This project exists to be an Easter Egg and to explain to beginners and intermediates in computing development how to use C#, .NET and WPF in a program to make a game like *Space Invaders*.

### Installing

* To be able to compile this project, it is very simple, you only need to download or clone the sources locally on your computer and open the solution from Visual Studio 2017 or higher (but you should open it from a lower version of Visual Studio by remaking the solution with your current version),
* Compile the project **SpaceInvaderGame**,
* After that you can execute the compiled binary *SpaceInvaderGame.exe* and enjoy the game.

## Deployment

To deploy the executable you only need to copy it into a machine with .NET Framework 4.0 installed.

## Authors

* **Stéphane Erhardt** - *Initial work* - [serhardt](https://github.com/serhardt)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Thanks to all contributors of open source materials which allow to inspire me!
