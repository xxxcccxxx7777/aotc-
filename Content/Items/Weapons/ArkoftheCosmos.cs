using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace AotC.Content.Items.Weapons
{
  internal class ArkoftheCosmos : ModItem
  {
    private Random rand = new Random();
    public int rnd;
    public float combo = 69f;
    public float charge;
    public bool stab;
    public static float MaxThrowReach = 650f;
    public static float SwirlBoltAmount = 6f;
    public static float SwirlBoltDamageMultiplier = 1f;

    public virtual void SetStaticDefaults()
    {
      this.DisplayName.SetDefault("The Ark of the Cosmos");
      this.Tooltip.SetDefault("A blade forged with pure magic and wielded by Hope itself");
      CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[this.Type] = 1;
    }

    public virtual void SetDefaults()
    {
      ((Entity) this.Item).width = 102;
      ((Entity) this.Item).height = 102;
      this.Item.useStyle = 5;
      this.Item.useTime = 15;
      this.Item.useAnimation = 15;
      this.Item.autoReuse = true;
      this.Item.DamageType = DamageClass.Melee;
      this.Item.damage = 2669;
      this.Item.knockBack = 9.5f;
      this.Item.crit = 15;
      this.Item.useTurn = true;
      this.Item.value = Item.buyPrice(2669, 0, 0, 0);
      this.Item.rare = 11;
      this.Item.UseSound = new SoundStyle?();
      this.Item.shoot = 10;
      this.Item.shootSpeed = 28f;
      this.Item.noMelee = true;
      this.Item.channel = true;
      this.Item.noUseGraphic = true;
    }

    public virtual void AddRecipes()
    {
      Mod calamity = AotC.AotC.Instance.Calamity;
      if (AotC.AotC.Instance.Calamity == null)
        return;
      Main.NewText("a", byte.MaxValue, byte.MaxValue, byte.MaxValue);
      Recipe recipe = this.CreateRecipe(1);
      recipe.AddIngredient(calamity.Find<ModItem>("DormantBrimseeker"), 1);
      recipe.AddIngredient(calamity.Find<ModItem>("SearedPan"), 1);
      recipe.AddIngredient(29, 9);
      recipe.AddIngredient(4923, 2);
      recipe.AddIngredient(154, 66);
      recipe.AddIngredient(calamity.Find<ModItem>("HyperiusBullet"), 92);
      recipe.AddIngredient(calamity.Find<ModItem>("CounterScarf"), 1);
      recipe.AddIngredient(calamity.Find<ModItem>("UndinesRetribution"), 1);
      recipe.AddIngredient(calamity.Find<ModItem>("ExoPrism"), 1);
      recipe.AddIngredient(1445, 1);
      recipe.AddIngredient(3364, 1);
      recipe.AddIngredient(calamity.Find<ModItem>("ShadowspecBar"), 5);
      recipe.AddTile(calamity.Find<ModTile>("DraedonsForge"));
      recipe.Register();
    }

    public virtual bool AltFunctionUse(Player player) => true;

    public virtual bool CanUseItem(Player player) => !((IEnumerable<Projectile>) Main.projectile).Any<Projectile>((Func<Projectile, bool>) (n => ((Entity) n).active && n.owner == ((Entity) player).whoAmI && n.type == ModContent.ProjectileType<ArkoftheCosmosSwungBlade>()));

    public virtual void NetSend(BinaryWriter writer) => writer.Write(this.charge);

    public virtual void NetReceive(BinaryReader reader) => this.charge = reader.ReadSingle();

    public virtual ModItem Clone(Item item)
    {
      ModItem modItem1 = ((ModType<Item, ModItem>) this).Clone(item);
      if (!(modItem1 is ArkoftheCosmos arkoftheCosmos) || !(item.ModItem is ArkoftheCosmos modItem2))
        return modItem1;
      arkoftheCosmos.charge = modItem2.charge;
      return modItem1;
    }

    public virtual bool Shoot(
      Player player,
      EntitySource_ItemUse_WithAmmo source,
      Vector2 position,
      Vector2 velocity,
      int type,
      int damage,
      float knockback)
    {
      if (player.altFunctionUse == 2)
        return false;
      if ((double) this.combo > 4.0)
      {
        this.combo = 0.0f;
        this.stab = true;
        this.rnd = this.rand.Next(1, 5);
      }
      ++this.combo;
      float num1 = (double) this.combo == 5.0 ? 2f : this.combo % 2f;
      if ((double) this.combo == (double) this.rnd && this.stab)
      {
        num1 = 3f;
        this.stab = false;
      }
      Projectile.NewProjectile((IEntitySource) source, ((Entity) player).Center, velocity, ModContent.ProjectileType<ArkoftheCosmosSwungBlade>(), damage, knockback, ((Entity) player).whoAmI, num1, this.charge);
      if ((double) num1 == 3.0)
      {
        float num2 = Utils.AngleTo(Vector2.op_Subtraction(((Entity) player).Center, Main.screenPosition), Main.MouseScreen);
        Projectile.NewProjectileDirect(((Entity) player).GetSource_FromThis((string) null), ((Entity) player).Center, Vector2.Zero, ModContent.ProjectileType<Beam>(), 1666, 1f, ((Entity) player).whoAmI, num2, 0.0f);
      }
      return false;
    }
  }
}
