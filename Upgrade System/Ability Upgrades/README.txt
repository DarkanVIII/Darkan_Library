How to use:

1. Create an enum containing all the abilities
2. Use the enum as generic type to derive a class from each base class contained in this folder (except AbilityDataBase)
3. Put the derived Upgrade Manager on a GameObject.
4. Use the CreateAssetMenu attribute to create a ScriptableObject Instance of UpgradeData
5. Derive a class from AbilityDataClass for each Ability and create ScriptableObjects from it
6. Fill the UpgradeData Dictionary with the created AbilityData
7. Create an Upgrade Canvas and create an empty AameObject with a Horizontal Layout Group that will hold the Ability slots
8. Create a prefab from the Ability Upgrade Slot prefab and assign the UpgradeSlot script and drag in the references
9. The button component of the UpgradeSlot needs to be assigned, Drag in the UpgradeSlot and call the Select() function
10. Go back to the Upgrade Manager and assign all references
SETUP DONE!!!

To start an Uograde Selection call the TryCreateUpgradeSelection() Method of the Upgrade Manager
To get a reference to the Selected Upgrade and to know when the selection is over subscribe to UpgradeSlot.OnFinishedSelection