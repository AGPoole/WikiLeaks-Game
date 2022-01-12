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

![Book logo](/Images/Typical-Game-Screen.png)
