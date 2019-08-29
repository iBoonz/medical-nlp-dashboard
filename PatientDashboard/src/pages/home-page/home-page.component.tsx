import * as React from "react";
import { searchPath } from "../search-page";
import { LogoJFKComponent } from "../../common/components/logo-jfk";
import { SearchButton } from "./components/search";
import { CaptionComponent } from "./components/caption";
import { SearchInput } from "./components/search";
import { FooterComponent } from "../../common/components/footer";
import { MenuItem, Select } from "material-ui";

const style = require("./home-page.style.scss");


interface HomePageProps {
  searchValue: string;
  selectedNihii: string;
  patients: any;
  handleChange: (event) => void;
  onSearchSubmit: () => void;
  onSearchUpdate: (newValue: string) => void;
}

export const HomePageComponent: React.StatelessComponent<HomePageProps> = (props) => {
  return (
    <div className={style.container}>
      <LogoJFKComponent classes={{container: style.logoContainer, svg: style.logoSvg}} />
      <div className={style.main}>
        <CaptionComponent />
        <Select className={style.select} 
        value={props.selectedNihii}
        onChange={props.handleChange}
      >
       <MenuItem key="-1" value="-1">Select A patient</MenuItem>

      {
        props.patients.map((patient, index) => (
          <MenuItem key={index} value={patient.value}>{patient.name}</MenuItem>
        ))
      }
      </Select>
        <SearchInput         
          searchValue={props.searchValue}
          onSearchSubmit={props.onSearchSubmit}
          onSearchUpdate={props.onSearchUpdate}
        />
        {
          props.selectedNihii != "-1" ?  <SearchButton onClick={props.onSearchSubmit}/> : <span></span>
        }
      </div>
      <FooterComponent className={style.footer}/>
    </div>
  )
};


