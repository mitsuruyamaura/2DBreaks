using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presenter : MonoBehaviour
{
    void Start()
    {
        // ReactiveProperty の利用しやすい状況は、MVP パターンと呼ばれる、UI の表示更新にかかわる処理
        // 値(得点)を購読(監視)することで、自動的に UI の表示更新を行うようなイベント処理を作り出すことができる

        // GameManager は Presenter という役割をもつ
        // Presenter とは、Model である ScoreManager と、View である UIManager の双方を知っていて、それをつなぐ役目をになう

        // ScoreManager に用意してある ReactiveProperty(Player と Enemy の得点を管理する値) をここで購読(監視)する命令を出す
        // いずれかの値が更新された場合、Subscribe メソッド内にある処理を自動的に実行する
        // 今回は値が更新されるたび、UIManager のメソッドを実行し、双方の値の情報を引数を使って提供する

        // こうすることで、Presenter 役である GameManager のみが、ScoreManager と UIManager を知っている状況(疎結合)でありながら
        // 値の更新に合わせて、画面の表示更新も一緒に連動して処理を行うことができる
        // よって、現在のように、値の更新に合わせてその都度、画面表示更新の命令を行う必要がなくなる


        // ReactiveProperty を購読　その①(始めに覚える方法)
        // Model として ScoreManager の代わりに GameData を利用しているが、これは、元の UpdateTxtScore メソッドの処理を活かしているため(メソッドに引数がないため)
        //GameData.instance.PlayerScore.Subscribe(_ => uiManager.PrepareUpdateTxtScore()).AddTo(this);
        //GameData.instance.EnemyScore.Subscribe(_ => uiManager.PrepareUpdateTxtScore()).AddTo(this);


        // ReactiveProperty を購読　その②(難しいが、覚えたい方法。①の処理をコメントアウトすれば、同じように正常に動きます)
        //Observable.CombineLatest(scoreManager.PlayerScore, scoreManager.EnemyScore, (playerScore, enemyScore) => (playerScore, enemyScore))
        //    .Subscribe(scores => uiManager.UpdateDisplayScoreObservable(scores.playerScore, scores.enemyScore))
        //   .AddTo(this);


        // まずは、ReactiveProperty を利用した MVP パターンによる、値と UI 表示更新の連動をしっかりと覚えること
        // ただし、MVP パターンは UI にのみ使うようにすること
        // Subscribe や AddTo といったメソッドの機能をしっかりと理解すること

        // プログラムには絶対な書き方はないので、UniRx においても、あくまでも、スキルの引き出しの幅を広げるものであると考えること
        // 頭でっかちにならないようにする。柔軟な思考を忘れない
        // ReactiveProperty 自体は色々な処理に応用可能だが、なんでもかんでも利用する、ということではない
        // 便利な機能であり、処理を書ける幅が広がるが、先ほども言っているように、すべてにおいて有効というわけではない(読み解けない人もいる)

        // 上記の実装例は、まずは、①の方で色々な処理を書いてみて、どういった処理が動くのかを試して、スラスラと書けるレベルにすることを目標にする

        // その後、②の処理の内容を理解していくようにする
        // ネットなどにも実装例はあるものの、自分のプロジェクトに落とし込んだものは絶対に見つからないので、処理の動きを覚えること
    }


}
