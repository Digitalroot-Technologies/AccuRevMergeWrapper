# AccuRevMergeWrapper

Sometime after AccuRev 4, the "merge -o -K" command was changed to return 1 if there were no files to merge. This causes an error when using nAnt to auto merge files. 

This program wraps the "merge -o -K" command, intercepts the output when there are no files to merge, and returns 0 if that is the case. This fixed the way nAnt handled the command.
