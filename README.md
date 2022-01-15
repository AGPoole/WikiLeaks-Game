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

### Algorithms

In this section, I'll go over 2 examples of algorithms I used in this project.

#### Hexagon Grid Distance

![](/Images/Hexagon-Grid.png)

The game takes place on a hexagon grid, indexed as shown above. Many features in the game would need to calculate the distance between different grid positions. This can obviously be done with any path-finding algorithm, but these would be O(n). I wanted an O(1) solution, similar to how you can find the distance in a regular square grid by adding the x difference and y difference.

To do this, observe that in the indexing in the image above, it is almost the same as a square grid. _(x, y)_ is adjacent to _(x-1, y)_, _(x+1, y)_, _(x, y+1)_ and _(x, y-1)_. In addition, if it is on an odd row, it is also adjacent to the 2 diagonally below (_(x-1, y-1)_ and _(x+1, y-1)_), while if it is on an even row it is adjacent to the 2 diagonally below. We will call the points on the odd rows _peaks_ while the points on even rows will be called _troughs_. Drawing this out as a grid, it looks like this:

![](/Images/Grid-As-Node-Graph.png)

From this, imagine you start at a _peak_ and want to get to another _peak_. Without loss of generality, we can assume the first coordinate is to the left and below the second (due to the symmetry of the graph and the fact that we can swap the two inputs). You can then see that the fast route will be a sequence of alternating right movements and diagonal up-right movements until you are in the same row or column as the target. If you are in the same row, you then can simply go right until you reach the target. If you are in the same column, you simply go up. We therefore need to calculate whether we'll reach the same row or column first. As the 2 moves we repeat overall increase the y coordinate by 1 and the x coordiate by 2, this is equivalent to calculating whether the overall x change between the source and the target is less than 2 times the target. If it is, the answer will be _xChange_+(_yChange_-(_xChange_/2) = _xChange_/2 + _yChange_. If not, it will be just be _xChange_, as each step will move you one across until you reach the target. The first case is shown on the left below while the other is shown on the right.

![](/Images/Possible-Routes.png)

Going from _trough_ to _trough_ is the same. For _trough_ to _peak_, observe that if you start one step down-left, it becomes equivalent to a _peak_ to _peak_ where the first move will take you to the actual starting position- you simply have to take away one from the answer. _Peak_ to _trough_ is the same, but you go up_right and add one.

The code for this is shown below:

```
public static int HexagonGridDistance(int iX1, int iY1, int iX2, int iY2)
{
    // start at bottom co-ordinate
    if (iY1 > iY2)
    {
        int iTempX = iX1;
        iX1 = iX2;
        iX2 = iTempX;

        int iTempY = iY1;
        iY1 = iY2;
        iY2 = iTempY;
    }

    // For same row or column, just go in a straight line
    if (iX1 == iX2)
    {
        return Math.Abs(iY1 - iY2);
    }
    if (iY1 == iY2)
    {
        return Math.Abs(iX1 - iX2);
    }

    bool bIs1Peak = Mod(iX1, 2) == 1;
    bool bIs2Peak = Mod(iX2, 2) == 1;

    // case 1: trough to trough or peak to peak
    if (bIs1Peak == bIs2Peak)
    {
        int iXDifference = Math.Abs(iX1 - iX2);
        int iYDifference = Math.Abs(iY1 - iY2);

        if (iXDifference < 2 * iYDifference)
        {
            // Move in across, up-across increments until below the target, then move up
            return (iXDifference / 2) + iYDifference;
        }
        else
        {
            // Move in across, up-across increments, then just move right
            return iXDifference;
        }
    }

    // case 2: peak to trough
    if (bIs1Peak && !bIs2Peak)
    {
        // To do this, we reduce to case 1 by moving diagonally down in the wrong direction.
        // This takes to a trough to trough where we know the first move would be diagonally
        // up. We can take away 1 from the result to then remove this

        int iXMoveAway = iX1 < iX2 ? -1 : 1;
        return HexagonGridDistance(iX1 + iXMoveAway, iY1 - 1, iX2, iY2) - 1;
    }

    // case 3: trough to peak
    // the first move will be diagonally up. Then we have case 1
    int iXMoveToward = iX1 < iX2 ? 1 : -1;
    return HexagonGridDistance(iX1 + iXMoveToward, iY1 + 1, iX2, iY2) + 1;
}
```

#### Distribution Generic

In this project, I wanted a more nuanced system for randomness than the in-built Unity Random library. I had multiple entities that would need to pick from lists of options (e.g. it could be a company deciding what strategy to use), where the probabilities would be different for different entries and would depend on their properties in complex ways. To do this, I created a generic class that allowed random sampling according to a given probability density function. The constructor for this class is as follows:

```
public Distribution(List<T> xOutcomes, Func<T, int, float> xPDf)
{
    m_xProbabilities = new List<(T, float)>();
    float fTotalProb = 0;


    for (int i = 0; i < xOutcomes.Count; i++)
    {
        float fNewValue = xPDf(xOutcomes[i], i);
        if (fNewValue >= 0f)
        {
            m_xProbabilities.Add((xOutcomes[i], fNewValue));
            fTotalProb += fNewValue;
        }
        else
        {
            Debug.LogError("Probability relative value less than 0. The calculations will not work for this");
        }
    }
    if (fTotalProb <= 0f || Mathf.Approximately(fTotalProb, 0f))
    {
        Debug.LogError("All probabilities are 0. This distribution will not work");
    }
    else
    {
        // Normalise the results
        for (int i = 0; i < m_xProbabilities.Count; i++)
        {
            m_xProbabilities[i] = (m_xProbabilities[i].Item1, m_xProbabilities[i].Item2 / fTotalProb);
        }
    }
}
```

To create a distribution, the player passes in the possible outcomes, as well as a function that calculates the probability of getting that outcome. The probability can also depend on the location in the list. The code then stores these in the member variable _m_xProbabilities_. It sums up the probabilities as it goes and divides each by the total at the end, to normalise the result (ensure it adds up to 1). This means we do not need to make the probability function does not need logic to make the total equal 1, which makes it more flexible.

Then, to sample, we simply choose a random number from 0 to 1 and sum through the probabilities in the list in order until it exceeds our random number:

```
public T Sample()
{
    float fValue = RandomRange(0f, 1f);
    float fRunningTotal = 0;
    for (int i = 0; i < m_xProbabilities.Count; i++)
    {
        fRunningTotal += m_xProbabilities[i].Item2;
        if (fRunningTotal > fValue)
        {
            return m_xProbabilities[i].Item1;
        }
    }
    return m_xProbabilities[m_xProbabilities.Count - 1].Item1;
}
```
