import { Component, getDebugNode } from '@angular/core';
import { NgStyle } from '@angular/common';
import {TwitterService} from './twitter.service'

@Component({
  selector: 'app-twitter',
  templateUrl: './app.twitter.html',
  styleUrls: ['./app.twitter.css']
})

export class TwitterComponent {
 
  tweets: any;
  constructor ( private twitterService:TwitterService){
   
  }

  ngOnInit() {
    this.getTweets();
  }

  getTweets() {
    this.tweets = this.twitterService.getTweet();

    window.setTimeout(() => {
       this.getTweets() ;
    }, 10000); 
  }
  getbg(Sentiments) {
    if (Sentiments >= 0 && Sentiments <= .20)
     return  "darkred";
    if (Sentiments >= .21 && Sentiments <= .40)
     return  "red";
    if (Sentiments >= .41 && Sentiments <= .55)
     return  "lightgrey";
    if (Sentiments >= .56 && Sentiments <= .70)
     return  "lightgreen";
    if (Sentiments >= .70 && Sentiments <= .90)
     return  "green";
    if (Sentiments > .90 )
     return  "darkgreen";
  };

  getfg(Sentiments) {
    if (Sentiments >= 0 && Sentiments <= .20)
     return  "white";
     if (Sentiments >= .21 && Sentiments <= .40)
     return  "white";
     if (Sentiments >= .41 && Sentiments <= .55)
     return  "black";
     if (Sentiments >= .56 && Sentiments <= .70)
     return  "black";
     if (Sentiments >= .70 && Sentiments <= .90)
     return  "white";
     if (Sentiments > .90 )
     return  "white";
  };


};
export class Tweet {
    author: string;
    post: string;
    sentiments : number;
};

