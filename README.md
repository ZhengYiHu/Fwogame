# Fwogame
<img width="800" src="https://github.com/ZhengYiHu/Fwogame/assets/33153763/ef256250-192f-4ddc-8201-a295fbec92c6">

## Where to play

The game is available on my [Itch](https://zyhu.itch.io/fwogame) or on my [website](https://zyhu.me/Fwogame)!

Source code available on my [Github](https://github.com/ZhengYiHu/Fwogame)

## Inspiration

The idea came from playing some of [Sokpop](https://sokpop.co/)'s little games, and I just wanted to make a small little project to replicate their signature leg animation as showcased in [this video](https://www.youtube.com/watch?v=2LbKuQsODHg).

![hf3nqb05e3a91](https://github.com/ZhengYiHu/Fwogame/assets/33153763/a217cf59-8c58-47bb-812c-c798cfe082d5)

## Procedural Limbs Animations

I used Unity's line renderer to create the limbs setting the origin on the character's body and the end at the target end effector which is going to be calculated according to various factors.

1. First of all, the end effector will anchor to the terrain and will not move when the body of the character and the origin of the limb moves around.
2. I set a maximum **Tollerance** range around each end effector.

   <img src="https://github.com/ZhengYiHu/Fwogame/assets/33153763/d384200a-eab1-4fa0-941e-6ae8907cceaa)" width="500">

3. a **Ray** is cast from each origin point downwards to determine how much the character has moved from the anchored point.
4. If the Raycast's hit point exceeded the Tollerance range, then the legs end effectors will be repositioned to the new target, making the caracter effectively perform a **step**.

   <img src="https://github.com/ZhengYiHu/Fwogame/assets/33153763/31ad6ab5-9e4b-4f56-871b-d3b93074e3c7" width="500">

5. A small offset and other adjustments are added for the walking cycle to look more natural.
6. More animations are then added to other parts of the character, like body rotations and damping based on the current velocity and so on.

   <img src="https://github.com/ZhengYiHu/Fwogame/assets/33153763/85b6024e-5711-4730-bc57-678c40223894)" width="500">

## More screenshots
<img width="400" alt="Fwogame1" src="https://github.com/ZhengYiHu/Fwogame/assets/33153763/b86aba67-04c0-4a88-925a-bfdc10d883d8">
<img width="400" alt="Fwogame2" src="https://github.com/ZhengYiHu/Fwogame/assets/33153763/d7f7d072-9332-4005-9fba-fefbb5f3c992">
<img width="400" alt="Fwogame3" src="https://github.com/ZhengYiHu/Fwogame/assets/33153763/08e2b4e6-e193-412c-ac4a-69774c8f6bed">
