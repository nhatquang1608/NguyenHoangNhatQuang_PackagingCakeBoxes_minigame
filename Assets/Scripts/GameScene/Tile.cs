using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public TileCell cell;
    public bool isCandy;
    public bool isCake;
    public bool isBox;

    public void Spawn(TileCell cell)
    {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }

    public async void MoveTo(TileCell cell)
    {
        if (this.cell != null) 
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        await Animate(cell);
    }

    public async void Merge(TileCell cell)
    {
        if(cell.tile.isCake)
        {
            if (this.cell != null) 
            {
                this.cell.tile = null;
            }
            this.cell = null;

            await Animate(cell);
            Destroy(cell.tile.gameObject);

            if (cell != null) 
            {
                cell.tile = this;
            }
            this.cell = cell;
        }
        else if(cell.tile.isBox)
        {
            if (this.cell != null) 
            {
                this.cell.tile = null;
            }
            this.cell = null;

            await Animate(cell);
            Destroy(gameObject);
        }
    }

    public async UniTask Animate(TileCell to)
    {
        transform.DOMove(to.transform.position, 0.1f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.moveSound);
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
    }
}
