import React, { Component } from 'react';
import MediaCard from './components/NewsCard';
import SearchAppBar from './components/SearchAppBar';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;
  constructor() {
    super();
    this.updateState = this.updateState.bind(this);
    this.state = {
      news: [],
      keyword: ""
    };
  }
  async componentWillMount(){

    const data = await fetch(`https://${window.location.host}/api/articles/all`);
    const articles = await data.json();
    this.setState({news: articles});
  }

  updateState(params) {
    console.log(`update state with ${params}`);
    this.setState({news: params});
  }

  render () {
    return (
      <div> 
        <div>
          <SearchAppBar keyword={this.state.keyword} updateState={this.updateState}/>
        </div>
        <div style={{position: 'absolute', left: '50%', transform: 'translate(-50%)'}}>
           {this.state.news.filter((item)=>item.urlToImage).map((article, index)=>{
               return <MediaCard key={index} article={article} />
           })}     
        </div>    
      </div>
    );
  }
}
