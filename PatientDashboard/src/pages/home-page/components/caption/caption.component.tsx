import * as React from "react"

const style = require("./caption.style.scss");


export const CaptionComponent = () => (
  <div className={style.caption}>
    <p className={style.title}>Search for medical content.</p>
    <p className={style.subtitle}>Let's find some medical info</p>
  </div>
);