# StoryScript

StoryBuilder uses text files to store dialogue, which you create using a scripting language called StoryScript. Its structure is similar to the Python programming language, but is much simpler and easier to read. This section introduces and explains StoryScript commands that you can use in your dialogue files.

Below is an example of a very basic dialogue file.

conversation.txt

```
registerSpeaker "Jessica"
registerSpeaker "Sharlene"

say "Sharlene" "I can't believe it's this easy!"
say "Jessica" "Yes, anyone can write a visual novel!"

endscript
```

## registerSpeaker

You usually begin a dialogue file by declaring the characters that participate in the conversation. Enter either one or two strings after the **registerSpeaker** command. The first string specifies the character name, and the second optional string assigns an image file to display. Place you speaker image files in the **\Assets\Resources\Portraits** folder of your project.

The following example creates a speaker named **Birman Donskoy**, and doesn't specify an image for the character.

```
registerSpeaker "Birman Donskoy"
```

The next examples creates the same speaker as above, this time associating the character with the **character1.png** image.

```
registerSpeaker "Birman Donskoy" "character1"
```

## say

The **say** command passes a string through the DialogueRenderer. You can optionally specify a speaker, which enables portrait switching, character name display, and changes to other options such as font and font color.

In the following example, Unity displays "**I'll ask her!**" on the screen without a specific speaker.

```
say "I'll ask her!"
```

In the next example, StoryBuilder sets the speaker to Jessica and Unity displays "**Hi there! How was class?**" on the screen.

```
say "Jessica" "Hi there! How was class?"
```

## endscript

The **endscript** command ends the dialogue and closes all related UI. Typically, you use endscript after the commands for each option of a choice, and at the end of each dialogue file.

## block and endblock

Use the **block** and **endblock** commands to enclose a portion of dialogue, for example, when you wish to jump to or skip that section of dialogue. This is useful for dialogue segments that repeat, and also helps with organization. Place the **block** command at the beginning of the dialogue segment, followed by the block name and a colon. Then, place the **endblock** command at the same indentation level where the dialogue ends. Remember to indent the text in between the two commands, as shown below.

```
block later:
    say "I can't get up the nerve to ask right now."
    say "With a gulp, I decide to ask her later."
    say "But I'm an indecisive person."
    say "I couldn't ask her that day."
    say "I end up never being able to ask her."
    say "I guess I'll never know the answer to my question..."
    say "<b>Bad Ending</b>."
    endscript
endblock
```

## jump

Use the **jump** command to move to a block with a given block name. In the following example, the code moves to a block called "later".

```
jump later
```

Be aware that after you use the **jump** command, it isn't possible to return to the original location of the jump.

## choice

In your story, you might decide to ask the player to make a selection that affects the sequence that follows, or even the outcome of the game. Use the **choice** command to display directions, list the selections available to the players, and control the actions that follow.

```
choice  "As soon as she catches my eye, I decide...":
    "To ask her right away.":
        jump rightaway
        endscript
    "To ask her later.":
        jump later
        endscript
```

In the above example, Unity displays the line "**As soon as she catches my eye, I decide...**" on the screen, and the player sees two options: "**To ask her right away.**" and "**To ask her later.**". If the player selects the first options, the game jumps to the block called "rightaway". If they select the second option, the game instead jumps to the block called "later".

## var

The **var** command lets you declare and set variables. Variables can be boolean, float, or string types. You don't need to declare the data type because StoryBuilder handles it internally. Below are some variable declaration examples.

```
var hungry = True
```

```
var age = 21
var book = "Twilight"
```

To change a variable, use the same format again.

```
var hungry = False
```

To use a variable, place its name in curly brackets ({}).

```
var nextage = {age} + 1
var sequel = "{book} Two"
```

## if

When you use the **if** command, the actions in its scope only run if its condition is fulfilled.

```
if hungry:
    say "I'm starving."
if hungry != True:
    say "No, thanks."
```

## print

Use the **print** command to output text to the Unity Editor's **Console** window. This is similar to [Debug.log](https://docs.unity3d.com/ScriptReference/Debug.Log.html) and is useful for debugging code.

```
if hungry:
    say "I'm starving."
    print "Character is hungry."
if hungry != True:
    say "No, thanks."
    print "Character is not hungry."
```

## run

Use the **run** command to run a user-defined function. StoryBuilder assumes that the function is a public void method of the dialogue file you're currently in.

## Command

Text that you type after a pound sign (#) are comments, which StoryBuilder ignores.

```
say "<b>Bad Ending</b>." # Uh oh.
# This is game over!
```

## Formatting

StoryBuilder support RichText markup tags. For more information, see the [Rich Text documentation](https://docs.unity3d.com/Manual/StyledText.html).

If you wish to use double quotation marks (") in a **say** statement, make sure that you precede them with a blackslash (\).

```
say "The book's title is \"Twilight.\""
```
