const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = {
    entry: {
        app: ['./Assets/Styles/site.scss', './Assets/Scripts/app.ts']
    },
    output: {
        path: path.join(__dirname, 'wwwroot/'),
        publicPath: '/',
        filename: '[name].js'
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: '[name].css'
        }),
        new CopyWebpackPlugin({
            patterns: [
                { from: 'Assets/assets', to: '.' }
            ]
        })
    ],
    module: {
        rules: [
            {
                test: /\.ts?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
                generator: {
                    filename: 'abc.js'
                }
            },
            {
                test: /\.(css|sass|scss)$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',
                    'sass-loader'
                ]
            },
            {
                // To use images on pug files:
                test: /\.(png|jpg|jpeg|ico)/,
                type: 'asset/resource',
                generator: {
                    filename: '[name][ext]'
                }
            },
            {
                // To use fonts on pug files:
                test: /\.(woff|woff2|eot|ttf|otf|svg)$/i,
                type: 'asset/resource',
                generator: {
                    filename: '[name][ext][query]'
                }
            },
            {
                // To use config:
                test: /\.(json)$/i,
                type: 'asset/resource',
                generator: {
                    filename: '[name][ext]'
                }
            }
        ]
    },
    devtool: 'inline-source-map',
    stats: 'errors-only',
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    }
};