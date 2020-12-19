# Mykaelos Unity Level Layout Generator

Level Layout Generator (LLG) is a cell-based random generator for level layouts that's definitely still a Work In Progress. After copying it to its third project, I knew it was time to make it into a shared library.
LLG most likely relies on shared code from [MUL](https://github.com/Mykaelos/MykaelosUnityLibrary), so you may need to include that too.

## How to add to your project

LLG is designed to be added to a Unity project as a git submodule. This allows me to make changes to LLG directly as I make new games and allows LLG to evolve over time with concrete use cases. The easiest way to add LLG is to just plop it down in the Assets folder.

```
cd UnityGameProjectFolder
git submodule add https://github.com/Mykaelos/MykaelosUnityLevelLayoutGenerator.git Assets/Libraries/MykaelosUnityLevelLayoutGenerator
```


## Authors

* **Mykaelos**
    * [Mykaelos on Github](https://github.com/Mykaelos)
    * [@Mykaelos](https://twitter.com/Mykaelos)
    * [Blog](http://www.mykaelos.com)

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE.md](LICENSE.md) file for details.

Pretty much do whatever you want with this code. It either came from me or CC0 sources. I can't promise any of it working, ever, and you use the code at your own risk. Also, most of this code will be in a constant state of flux, because I change it as my projects evolve. So this project may be better used as examples of ways to solve specific coding problems, and as a starting point to your own better solutions.
