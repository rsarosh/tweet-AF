import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response } from '@angular/http';
import { catchError, map, tap } from 'rxjs/operators';
import {Observable} from 'rxjs/Rx';

// Import RxJs required methods
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

export class Tweet {
    author: string;
    post: string;
    sentiments : number;
};


@Injectable()
export class TwitterService {
  constructor(private http: HttpClient) {
      this.tweets = new Array();
  }
  tweets: object[] ;
 


  getTweetFromService (): Observable<{}| Tweet[]>{
    return  this.http.get <Tweet[]> ("http://localhost:59767/api/tweet").pipe(
        tap(_  => this.tweets.unshift(..._))
      );
    
        
  }

  getTweet () {
      let max_length = 100;
      this.getTweetFromService ().subscribe (d => 
        { 
            console.log ("==>" +  d); //If I dont subscribe then the promise is not fullfilled.
        })

        if (this.tweets.length > max_length) {
            this.tweets.splice(max_length, this.tweets.length - max_length);
        }
  
        return this.tweets ;

     

      /*
       return  [
        { Author: "AAA", Post: 'Mr. Nice', Sentiments:4 },
        { Author: "BBBB", Post: 'There are many different ways to declare ng-class. Take a look here. This useful SO post talks about different ways to conditionally applying css in AngularJS.', Sentiments:32 },
        { Author: "CCCCCC", Post: 'Make sure you’ve got the right web address: https://embed.plnkr.co', Sentiments:64 },
        { Author: "DDDD", Post: 'Mr. NiCAce', Sentiments:78 },
        { Author: "EEEEE", Post: 'Mr. NUILice', Sentiments:94 }
      ];
      */
   }
}