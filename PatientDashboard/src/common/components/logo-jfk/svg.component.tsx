import * as React from 'react';
const styles = require('./style.scss');

interface Props {
  className: string;
}

export const JFKSvg: React.StatelessComponent<Props> = (props) => (
  <svg
    className={`${styles.logo} ${props.className}`}
    xmlns="http://www.w3.org/2000/svg"
    xmlnsXlink="http://www.w3.org/1999/xlink"
    x="0px"
    y="0px"
    viewBox="0 0 169.9 169.9"
    xmlSpace="preserve"
  >

  </svg>
)
