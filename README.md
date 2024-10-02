# ゲームタイトル / Game Title / 游戏名称

**ZombieShooting**

---

## ゲーム概要 / Game Overview / 游戏简介

**日本語：**  
**ZombieShooting** は、時間の都合で開始画面や終了画面の UI が未実装です。ゲームの終了は `Alt + F4` で行ってください。本ゲームの開発は、主にゲームプレイの中核機能に焦点を当てており、プレイヤーは3種類の武器を使い分けて、襲い来るゾンビを倒します。ゾンビは通常はゆっくり移動しますが、プレイヤーが近づくと急に加速し、Unity のナビゲーション機能を使ってプレイヤーを追尾します。また、ゾンビの生成はオブジェクトプールを活用して、パフォーマンスを最適化しています。

**English:**  
**ZombieShooting** does not include start or end UI screens due to time constraints. To close the game, use `Alt + F4`. The development focused primarily on core gameplay mechanics. The player can switch between 3 different weapons to fend off waves of zombies. Zombies normally move slowly but will accelerate and track the player when they get close, utilizing Unity's navigation system. The game also employs object pooling to optimize performance and ensure smoother gameplay.

**中文：**  
由于时间原因，《ZombieShooting》没有加入开始和结束的 UI 界面，关闭游戏请使用 `Alt + F4`。本次开发主要专注于核心游戏玩法功能，玩家可以在三种不同的武器之间切换来对抗一波又一波的僵尸。僵尸通常缓慢移动，但当玩家接近时，它们会加速并利用 Unity 的导航系统追踪玩家。此外，游戏通过使用对象池技术优化了僵尸的生成，提高了性能。

---

## 操作方法 / Controls / 操作说明

**日本語：**  
- 移動：`W` `A` `S` `D` キーで移動  
- ジャンプ：スペースキーでジャンプ  
- ダッシュ：`Shift` キーを押し続けてダッシュ  
- 射撃：左クリックで射撃  
- 武器を覗く：スコープ付きの武器の場合、右クリックで覗き込み  
- 武器の切り替え：数字キー `1` `2` `3` で武器を切り替え  
  - `1`：スナイパーライフル（右クリックでスコープ、一撃でゾンビを倒せる、複数のゾンビに貫通ダメージを与えられる）  
  - `2`：ハンドガン（4発でゾンビを倒す）  
  - `3`：ナイフ（2回の攻撃でゾンビを倒し、複数のゾンビに同時にダメージを与える）

**English:**  
- Movement: `W` `A` `S` `D` keys to move  
- Jump: Spacebar to jump  
- Sprint: Hold `Shift` to run faster  
- Shooting: Left-click to shoot  
- Aim Down Sights: Right-click to aim (for scoped weapons)  
- Switch Weapons: Use the number keys `1` `2` `3` to switch between weapons  
  - `1`: Sniper Rifle (Right-click to aim, can kill zombies in one shot, deals piercing damage to multiple zombies)  
  - `2`: Handgun (Takes 4 shots to kill a zombie)  
  - `3`: Knife (Takes 2 hits to kill a zombie, can damage multiple zombies at once)

**中文：**  
- 移动：使用 `W` `A` `S` `D` 键移动  
- 跳跃：按空格键跳跃  
- 加速：按住 `Shift` 键加速奔跑  
- 射击：点击鼠标左键射击  
- 瞄准：右键瞄准（适用于带瞄准镜的武器）  
- 切换武器：使用数字键 `1` `2` `3` 切换武器  
  - `1`：狙击枪（右键瞄准，可一枪击杀僵尸，并可对多个僵尸造成穿透伤害）  
  - `2`：手枪（需要四枪才能击杀僵尸）  
  - `3`：刀（两刀可以击杀僵尸，能同时对多个僵尸造成伤害）

---

## ゲームの特徴 / Gameplay Mechanics / 游戏机制

**日本語：**  
- ゾンビは通常、ゆっくりと移動しますが、プレイヤーが近づくと急加速し、Unity のナビゲーション機能を使ってプレイヤーを追尾します。ゾンビの生成にはオブジェクトプールが使用されており、ゲームのパフォーマンスを向上させています。プレイヤーは状況に応じて 3 種類の武器を切り替えながら、生き延びるためにゾンビを撃退する必要があります。

**English:**  
- Zombies normally move slowly, but when the player gets close, they will accelerate and track the player using Unity's navigation system. The game uses object pooling for zombie generation, improving performance and efficiency. The player must switch between the three available weapons to survive the onslaught of zombies.

**中文：**  
- 僵尸通常缓慢移动，但当玩家靠近时，它们会加速并利用 Unity 的导航系统追踪玩家。游戏通过对象池生成僵尸，提高了游戏性能和效率。玩家需要在三种武器之间进行切换，抵御僵尸的进攻以求生存。
"""

# Saving the content as a markdown file
with open("/mnt/data/ZombieShooting_README.md", "w") as file:
    file.write(content)

"/mnt/data/ZombieShooting_README.md"
