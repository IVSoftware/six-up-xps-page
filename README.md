This is an interesting question with a lot of nuances. You'll have to forgive me if my example or approach isn't an "exact" fit for what you're doing. However, you asked it this way:

>Is there any better way to get this done?

If I'm following your code and intent, then yes the better way would be making sure that the _document_ (the data) is being properly decoupled from the view that displays it. because I don't believe you should be have to clone anything, but especially not framework elements. From reading your code it implies the existence of some knd of record that might look similar to this portable representation:
