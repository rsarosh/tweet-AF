import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { TwitterComponent } from './app.twitter';
import { HttpClientModule } from '@angular/common/http';
import {TwitterService} from './twitter.service'

@NgModule({
  declarations: [
    AppComponent, TwitterComponent
  ],
  imports: [
    BrowserModule, HttpClientModule
  ],
  providers: [TwitterService],
  bootstrap: [AppComponent, TwitterComponent]
})
export class AppModule { }
