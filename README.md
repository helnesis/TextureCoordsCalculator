# Texture Coords Calculator GUI

## Overview
Calculates texture coordinates automatically, by selecting off the desired area directly through your .BLP file. If you are bored to do this manually, so this little application is specially designed for you :)



## Requirements for compiling

Since it used C# and WPF, some dependencies are required before using this project. Make sure you have the following :

- [Visual Studio](https://visualstudio.microsoft.com/), [Visual Studio Code](https://code.visualstudio.com/) or your favorite IDE.

- [.NET SDK](https://dotnet.microsoft.com/download)

- [Git](https://git-scm.com/)

## Build instructions

1. **Clone the repository** :

```sh
git clone https://github.com/helnesis/TextureCoordsCalculator.git
```

3. **Navigate to the project directory, get the submodule** :

```sh
cd ./TextureCoordsCalculator.git
git submodule update --init --recursive
```

You're now ready to compile the project, through your favorite IDE or ``dotnet build``

## Usage
It is quiet easy, open ``TextureCoordsCalculatorGUI.exe``, open your texture, select the desired area. Coords will appear!


## Example

![Texture Coords Calculator example](img/app.jpeg)
