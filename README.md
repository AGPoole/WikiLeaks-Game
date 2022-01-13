# Game Overview

## Introduction

This document details a video game I have been developing in my spare time, based on recent computer hacking and technology scandals. It includes a simple economic simulation. The player takes the role of a computer hacker and then interacts with the various government agencies, characters and companies featured in the simulation, to make money, influence politics or cause chaos.

The purpose of this document is to give an overview of the key features of the project. The project was created in Unity Engine with C# and is currently on hold. I felt an explanation would be useful as the code is not widely documented or self-explanatory (much of it interacts with the Unity systems and is hard to follow on its own).

### Goals

These are the main goals I had when I began the project:

<ul>
<li> Improve my software engineering skills. When I was working on it, I was at a period in my job where the majority of my work was bug fixing. While I did enjoy this, I felt it would be useful to also work on designing larger systems. This would prepare me for my later career and help me build on the skills I developed at university. For this project, I needed to design a larger system of multiple interacting classes. I also included some automated testing and logging systems which weren't part of my job at the time. </li>
<li> Develop an easily modifiable and extendable system. It's difficult at the start of a project to plan it all out, as there are many trade-offs that are hard to figure out. For instance, when deciding how complex the simulation will be, there can be issues if you go to far in either direction. A very complex simulation will be hard for the player to reason about, while a very simple one will be easy to exploit or lack interesting ways to interact with it. You can't tell easily what will feel right before developing the project, so I wanted to make it easy to change in drastic ways. This would also make it possible to have different settings and modes to suit different player preferences.<br />
I also wanted to be able to add new features as easily as possible. I wanted the player to have a wide range of tools to interact with the world. I would inevitably come up with new ideas while working on or testing the game, so wanted to be able to include those as easily as possible. </li>
<li> Build an economic simulation complex enough for players to come up with their own schemes and heists. To give a few examples:
  <ul>
  <li> The player may buy shares in a company and then use their hacking skills to sabotage its rivals, so that its share price increases. They could also blackmail politicians to support policies that benefit the company.
  <li> They may create social media bots to increase the popularity of a political party that supports less restrictive cyber-security laws. Then, once the party is in power, they will be able to attack targets that were previously well protected.
  <li> A company may hire the player to improve their cyber-security. The probability and reward for this may depend on how recently they have been attacked. The player may notice this and attack the company to drive demand for cyber-security up.
  <li> The player may be able to disguise themselves as a country or company when attacking different entities. They could then cause conflicts between different countries/companies, that would weaken both making them more vulnerable to the player. They could also use this skill to get rid of organisations that threaten them.
</ul>
With a simulation where different entities interact in complex ways, the player should be able to come up with interesting and original ways to achieve their goals. They may notice side-effects to their actions that they can use to their benefit. I feel that this allows for open and creative problem-solving, which is more interesting than just having a set of scripted ways to interact with the game.
</li>
</ul>

## Code Overview

![](/Images/Typical-Game-Screen.png)

Above is a screen-shot of the game in its current state. It takes place on a hexagon grid, made up of several governments and companies (we will call these collectively "organisations"). These then consist of several systems, which are the buildings shown on the screen. These can have different roles (e.g. data-science, cyber-security, military) and can be traded between the different organisations as they gain and lose money. The white lines dividing up the grid show where the areas owned by each organisation. The black buildings are government while the blue ones are companies. The player can hack into different systems (the ones currently hacked have their text in green) and can then hack adjacent ones. The shields indicate how difficult it will be to hack them. The icons in the bottom left are the skills available to the player, while the bar at the top show the player's current statistics (e.g. how much money they have).

The systems then grant different bonuses and abilities to the organisation that owns them, as well as the player if they hack it. They will increase or decrease in size as their owner gains or loses money, with images as shown below.

![](/Images/Building-Levels.png)

### Structure of the code

![](/Images/Organisation-and-System-UML.png)

As shown in the above UML diagram, both the systems and organisations inherit from abstract base classes, which include an *OnNextTurn* function that dictates what to when updated. The organisation stores a list of systems which it calls _OnNextTurn_ on to update within its own _OnNextTurn_.

![](/Images/Organisation-UML.png)

Each organisation also references 2 corresponding classes, a _Values_ class and a _Data_ class. These also exist for the systems, although I haven't needed _Data_ classes for some of them yet.

The _Values_ classes contains settings for the organisation, so that they can be viewed and editted in the Unity editor. There is only one instance, so I can adjust the values for all governments or companies at once. This seems like something that should be handled with static variables, but these cannot be seen in the Unity editor and therefore I cannot easily edit them during gameplay.

The _Data_ classes contain the logic for the class, separate from the UI. The reason I wanted this was so that you could duplicate the game's state and run it ahead for several turns to predict what will happen, without having to do costly UI updates. The game is entirely deterministic (I.E. there is no randomness in the logic), so you can guarentee that when you do this, your predictions will be accurate (unless the player interferes). This has several benefits:

* You can quickly run tests outside the game to see what happens. For instance, I could adjust a parameter and then run a quick simulation of the game to measure what how the world is likely to turn out.
* When elections happen in game, it can simulate the world with each party's policies in 20 turns time, to see which outcome is better. This can then be factored in to deciding which people vote for, along with the influence of the player and the other organisations.
* Companies can decide on their actions by running simulations with the different options. This means you want need complex AI logic that understands how the game works, as they can just calculate which of their options is best for them. This also means you would not need to update AI logic as you update the game.
* After the game is complete, you can run it quickly from the start to calculate how the world would be without the player's interaction. You could then display the differences to the player. For instance, it may tell them that without them, there would have a been a nuclear war or financial collapse.

The _Data_ classes include a _ShallowCopy_ function to get a duplicate. They know whether they are clones and have slightly different logic if they are, to prevent infinite loops where the clones duplicate the world's data to simulate the future.

### Other Features

The code includes a range of abstract classes for creating new instances of different features:

* _PerkBase_ for the abilities you unlock when hacking systems
* _WeaponBase_ for the weapons you can use. This includes a type parameter for what the weapon can target and includes abstract methods for its effects, conditions on usage and the time it takes to recharge
* _MissionBase_ for tasks and contracts given to you by different organisations and characters. This includes methods for detecting completion and describing the reward

There is also a notification system, to display information to the player. I also use this to show debug info, as I have more control over it than the in-built Unity systems. For instance, I can make it display extra information when the mouse hovers over.

In addition, there is a system to log data (e.g. what the government sets the tax level at) to a .csv file so I can graph it and see what is going on.
