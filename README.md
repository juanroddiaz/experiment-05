Intro
=====

We are given some code from a coworker who wants to implement observability on the users game state.

The simple cases work fine but we are facing a problem when multiple properties are changed. 
This is demonstrated in a test case `TestSuite.CanObserveConsistentGameStateChanges` that you can run in the
test runner within unity.

The issue is that observers will get called while the game state is within a transaction. So they will
observe in-between values.

Objective
=========

Main objective: Implement a system that supports observability on game state properties but makes sure 
that observers will only see consistent game states. 

The main objective should be achieved for all (future) properties. So the goal is not to find a solution
that will only work for the specific problem shown in the test.
	[JD] My solution is a little bit more verbose than just registering actions and call the OnValueChanged
	directly but it helps to solve a lot of potential problems with data coherency in profile. Also it ensures
	not repeated actions to be executed and only the correct onValueChanged associated actions to be called.

This means that we want to be able to register multiple observers for different game state properties 
and they should all be called only after the game state is in a consistent state but only when a property
changed that they actually observe. So calling all observers for all changes to the game state is not 
good enough.
	[JD] Check CanSelectivelyObserveConsistentGameStateChanges test case

An interesting additional feature could be that observers could register for multiple properties but are only 
notified once even when both are changed.
	[JD] Check CanUseSameObserverConsistentGameStateChanges test case: when the same observer is registered for
	multiple properties, if one of them is modified it's called just once

Expectations
============

The main point of the assignment is to get an idea of your skills to design and implement a solution. 
So please don't try to create something that just barely works with the minimal amount of lines of code. 
That would defeat the purpose.
	[JD] A simple yield/async wait for end of frame could syncronize all the data changed in several operations
	during one single frame: I went for that solution at first just to check if the test was solved that way (it
	was)... but yep, too simplistic and involves calling to a general validator for all variables.

Any solution that would fix the broken test case is a valid one. You are allowed to change the API 
as long as the main objective is accomplished: Being able to observe property changes when the game state
is in a consistent state.	

Take into consideration that a real game state will be much more complex. So it will contain a lot 
more properties. It will also contain more complex substructures like lists and non-trivial types.
	[JD] I really wanted to check a potential list with ints just to mess around with changes on entries and	
	other gameState values. Obviously a more realistic approach would use a serializable class to store data but
	for the test I think the <int, int> validator is enough. Check CanObserveConsistentCharacterGameStateChanges
	test case.

Please concentrate on the observable problem and ignore other issues with the code. I.e. don't introduce 
dependency injection just to get rid of the singletons.

Your solution should be something that you would actually propose to your team. So it should have 
the quality you expect from a pull request. We will evaluate your code in this way. So we will assume that 
this is an actual real world sample of what you would want to check into the repository. 
	[JD] The idea is to keep method coherency across all the gameState and it's potential new fields: it
	requires a method to subscribe actions per field and a "queue action to be executed after value changed"
	call, but it ensures that after calling ExecuteActions you don't have to worry about unconsistent data or
	about calling unnecessary validators, as the logic only queues the ones you need for the current frame's
	operations. Also, this method gives you a simple solution for potential async data modification: you just
	add the ExecuteActions method to the Response callback.

This is really important:

When we look at your code we will not think: Ah you would not have pushed this in a real project.
We will take this as it is.

So please make sure that you do the best you can do. Take your time. We know that you might have other
things to do. So it is entirly ok if you submit your result 7 days after you got the assignment.
If you need more time please just drop us a note so that we know that you are still interested.
	[JD] I explored some Observers solutions around internet and almost all of them go with the Interface
	solution, that actually depends a lot of type checks and more abstract usage of methods (reflection... not
	sure if I'd like that for every potential change on the player data), and to try to accomplish a more to the
	bone implementation I'd rather accumulate actions in a list specifying the target profile's value to
	associate. For a more general Event/Observe implementation I'd add more "free" subscriptions and singletons
	but this wasn't the case, and maybe thinking about how to simplify the regsitration when adding new
	variables to the profile.

Thanks and happy coding!

We are looking forward to your solution and hopefully to meet you soon!
	[JD] Thanks! I hope we have the chance to discuss about the solution and to meet soon!
